using BillingService.Models;
using Microsoft.Extensions.Logging;
using Android.BillingClient.Api;
using AndroidBillingResult = Android.BillingClient.Api.BillingResult;

namespace BillingService.Services;

public class BillingService : BaseBillingService
{
    private BillingClient? _billingClient;
    private readonly object _lockObject = new();
    private BillingClientStateListener? _stateListener;
    private PurchasesUpdatedListener? _purchaseListener;

    public BillingService(ILogger<BaseBillingService> logger) : base(logger)
    {
        InitializeListeners();
    }

    private void InitializeListeners()
    {
        _stateListener = new BillingClientStateListener(this);
        _purchaseListener = new PurchasesUpdatedListener(this);
    }

    protected override async Task<bool> InitializePlatformAsync()
    {
        return await Task.Run(() =>
       {
           try
           {
               var context = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
               if (context == null)
               {
                   _logger.LogError("No current activity available for billing initialization");
                   return false;
               }

               if (_purchaseListener == null)
               {
                   _logger.LogError("Purchase listener not initialized");
                   return false;
               }

               var pendingPurchasesParams = PendingPurchasesParams.NewBuilder()
                   .EnableOneTimeProducts()
                   .Build();

               _billingClient = BillingClient.NewBuilder(context)
                   .SetListener(_purchaseListener)
                   .EnablePendingPurchases(pendingPurchasesParams)
                   .Build();

               _logger.LogInformation("Starting billing client connection...");
               if (_stateListener != null)
               {
                   _billingClient.StartConnection(_stateListener);
               }
               return true;
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Failed to initialize billing client");
               return false;
           }
       });
    }

    protected override async Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts)
    {
        try
        {
            var updatedProducts = new List<Product>();

            // Create product list for querying
            var productList = new List<QueryProductDetailsParams.Product>();

            foreach (var product in baseProducts)
            {
                var queryProduct = QueryProductDetailsParams.Product.NewBuilder()
                    .SetProductId(product.Id)
                    .SetProductType(BillingClient.ProductType.Inapp)
                    .Build();

                productList.Add(queryProduct);
            }

            if (productList.Count == 0)
            {
                _logger.LogWarning("No products to query");
                return baseProducts;
            }

            var queryParams = QueryProductDetailsParams.NewBuilder()
                .SetProductList(productList)
                .Build();

            _logger.LogInformation("Querying product details for {Count} products", productList.Count);

            if (_billingClient == null)
            {
                _logger.LogError("Billing client is null when querying product details");
                return baseProducts;
            }

            var productResult = await _billingClient.QueryProductDetailsAsync(queryParams);

            if (productResult.Result.ResponseCode == BillingResponseCode.Ok)
            {
                _logger.LogInformation("Successfully retrieved {Count} product details", productResult.ProductDetails.Count);

                // Create a dictionary for quick lookup of ProductDetails by ID
                var productDetailsDict = productResult.ProductDetails.ToDictionary(pd => pd.ProductId, pd => pd);

                foreach (var baseProduct in baseProducts)
                {
                    var updatedProduct = new Product
                    {
                        Id = baseProduct.Id,
                        Name = baseProduct.Name,
                        Description = baseProduct.Description,
                        Price = baseProduct.Price,
                        PriceAmount = baseProduct.PriceAmount,
                        IsOwned = baseProduct.IsOwned,
                        ImageUrl = baseProduct.ImageUrl
                    };

                    // Update with actual product details from Google Play
                    if (productDetailsDict.TryGetValue(baseProduct.Id, out var productDetails))
                    {
                        updatedProduct.Name = productDetails.Name ?? baseProduct.Name;
                        updatedProduct.Description = productDetails.Description ?? baseProduct.Description;

                        var formattedPrice = GetFormattedPrice(productDetails);
                        if (!string.IsNullOrEmpty(formattedPrice))
                        {
                            updatedProduct.Price = formattedPrice;
                        }

                        var priceAmount = GetPriceAmount(productDetails);
                        if (priceAmount.HasValue)
                        {
                            updatedProduct.PriceAmount = priceAmount.Value;
                        }

                        // Check if this product is owned
                        updatedProduct.IsOwned = _ownedProducts.Contains(baseProduct.Id);

                        _logger.LogDebug("Updated product {ProductId}: {Name} - {Price}",
                            updatedProduct.Id, updatedProduct.Name, updatedProduct.Price);
                    }
                    else
                    {
                        _logger.LogWarning("Product details not found for {ProductId}, using base product info", baseProduct.Id);
                        // Still check if owned even if details not found
                        updatedProduct.IsOwned = _ownedProducts.Contains(baseProduct.Id);
                    }

                    updatedProducts.Add(updatedProduct);
                }

                return updatedProducts;
            }
            else
            {
                _logger.LogError("Failed to query product details: {ResponseCode} {DebugMessage}",
                    productResult.Result.ResponseCode, productResult.Result.DebugMessage);

                // Even if query fails, update ownership status
                foreach (var product in baseProducts)
                {
                    product.IsOwned = _ownedProducts.Contains(product.Id);
                }

                return baseProducts;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying platform products");

            // Update ownership status even on exception
            foreach (var product in baseProducts)
            {
                product.IsOwned = _ownedProducts.Contains(product.Id);
            }

            return baseProducts;
        }
    }

    protected override async Task<List<string>> GetPlatformPurchasedProductsAsync()
    {
        try
        {
            var purchasedProducts = new List<string>();
            var tcs = new TaskCompletionSource<List<string>>();

            var queryPurchasesParams = QueryPurchasesParams.NewBuilder()
                .SetProductType(BillingClient.ProductType.Inapp)
                .Build();

            var purchaseResponseListener = new PurchasesResponseListener((billingResult, purchases) =>
            {
                if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
                {
                    foreach (var purchase in purchases)
                    {
                        if (purchase.PurchaseState == PurchaseState.Purchased)
                        {
                            purchasedProducts.AddRange(purchase.Products);
                        }
                    }
                }
                tcs.SetResult(purchasedProducts);
            });

            if (_billingClient != null)
            {
                _billingClient.QueryPurchases(queryPurchasesParams, purchaseResponseListener);
            }
            else
            {
                _logger.LogError("Billing client is null when querying purchases");
                tcs.SetResult(purchasedProducts);
            }

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying purchased products");
            return new List<string>();
        }
    }

    protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
    {
        try
        {
            var productList = QueryProductDetailsParams.Product.NewBuilder()
            .SetProductType("InApp")
            .SetProductId(productId)
            .Build();

            var activity = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (activity == null)
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = "No current activity available"
                });
            }

            var productDetailsParams = QueryProductDetailsParams.NewBuilder().SetProductList(new[] { productList });

            if (_billingClient == null)
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = "Billing client is not initialized"
                });
            }

            var productResult = await _billingClient.QueryProductDetailsAsync(productDetailsParams.Build());

            var skuDetails = productResult.ProductDetails.FirstOrDefault() ?? throw new ArgumentException($"{productId} does not exist");
            BillingFlowParams.ProductDetailsParams productDetailsParamsList;
            productDetailsParamsList = BillingFlowParams.ProductDetailsParams.NewBuilder()
            .SetProductDetails(skuDetails)
            .Build();
            var billingFlowParams = BillingFlowParams.NewBuilder()
                .SetProductDetailsParamsList(new[] { productDetailsParamsList })
                .Build();

            var billingResult = _billingClient.LaunchBillingFlow(activity, billingFlowParams);

            if (billingResult.ResponseCode == BillingResponseCode.Ok)
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = true,
                    ProductId = productId,
                    ErrorMessage = ""
                });
            }
            else
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = $"Purchase failed: {billingResult.DebugMessage}"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during purchase flow for product {ProductId}", productId);
            return await Task.FromResult(new PurchaseResult
            {
                IsSuccess = false,
                ProductId = productId,
                ErrorMessage = ex.Message
            });
        }
    }

    protected override async Task<bool> RestorePlatformPurchasesAsync()
    {
        try
        {
            var restoredPurchases = await QueryExistingPurchasesAsync();
            _logger.LogInformation("Restored {Count} purchases", restoredPurchases.Count);

            // Process each restored purchase
            foreach (var purchase in restoredPurchases)
            {
                ProcessPurchase(purchase);
            }

            return restoredPurchases.Count >= 0; // Return true even for 0 purchases (successful query)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring purchases");
            return false;
        }
    }

    #region Internal Event Handlers

    internal void OnBillingServiceDisconnected()
    {
        _logger.LogInformation("Billing service disconnected");

    }

    internal void OnBillingSetupFinished(AndroidBillingResult billingResult)
    {
        var responseCode = billingResult.ResponseCode;
        var debugMessage = billingResult.DebugMessage;

        _logger.LogInformation("Billing setup finished: {ResponseCode} {DebugMessage}", responseCode, debugMessage);

    }

    internal void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases)
    {
        if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
        {
            _logger.LogInformation("Purchase updated: {Count} purchases", purchases.Count);

            foreach (var purchase in purchases)
            {
                ProcessPurchase(purchase);
            }
        }
        else if (billingResult.ResponseCode == BillingResponseCode.UserCancelled)
        {
            _logger.LogInformation("User canceled purchase");
        }
        else
        {
            _logger.LogError("Purchase failed: {ResponseCode} {DebugMessage}",
                billingResult.ResponseCode, billingResult.DebugMessage);
        }
    }

    #endregion

    #region Helper Methods
    private async Task<List<Purchase>> QueryExistingPurchasesAsync()
    {
        try
        {
            var queryParams = QueryPurchasesParams.NewBuilder()
                .SetProductType(BillingClient.ProductType.Inapp)
                .Build();

            if (_billingClient == null)
            {
                _logger.LogWarning("Billing client is null");
                return new List<Purchase>();
            }

            var purchasesResult = await _billingClient.QueryPurchasesAsync(queryParams);
            _logger.LogInformation("Successfully queried {Count} existing purchases", purchasesResult.Purchases.Count);
            return purchasesResult.Purchases.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying existing purchases");
            return new List<Purchase>();
        }
    }

    private void ProcessPurchase(Purchase purchase)
    {
        try
        {
            if (purchase.PurchaseState == PurchaseState.Purchased)
            {
                // Add to owned products
                foreach (var productId in purchase.Products)
                {
                    _ownedProducts.Add(productId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing purchase");
        }
    }

    private string? GetFormattedPrice(ProductDetails productDetails)
    {
        try
        {
            var offerDetails = productDetails.GetOneTimePurchaseOfferDetails();
            return offerDetails?.FormattedPrice;
        }
        catch
        {
            return null;
        }
    }

    private decimal? GetPriceAmount(ProductDetails productDetails)
    {
        try
        {
            var offerDetails = productDetails.GetOneTimePurchaseOfferDetails();
            var priceAmountMicros = offerDetails?.PriceAmountMicros;
            if (priceAmountMicros != null)
            {
                return priceAmountMicros.Value / 1_000_000m;
            }
        }
        catch
        {
            // Fallback to default
        }
        return null;
    }
    #endregion
}

#region Listener Classes

internal class BillingClientStateListener : Java.Lang.Object, IBillingClientStateListener
{
    private readonly BillingService _service;

    public BillingClientStateListener(BillingService service)
    {
        _service = service;
    }

    public void OnBillingServiceDisconnected()
    {
        _service.OnBillingServiceDisconnected();
    }

    public void OnBillingSetupFinished(AndroidBillingResult billingResult)
    {
        _service.OnBillingSetupFinished(billingResult);
    }
}

internal class PurchasesUpdatedListener : Java.Lang.Object, IPurchasesUpdatedListener
{
    private readonly BillingService _service;

    public PurchasesUpdatedListener(BillingService service)
    {
        _service = service;
    }

    public void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases)
    {
        _service.OnPurchasesUpdated(billingResult, purchases);
    }
}

internal class PurchasesResponseListener : Java.Lang.Object, IPurchasesResponseListener
{
    private readonly Action<AndroidBillingResult, IList<Purchase>> _onResponse;

    public PurchasesResponseListener(Action<AndroidBillingResult, IList<Purchase>> onResponse)
    {
        _onResponse = onResponse;
    }

    public void OnQueryPurchasesResponse(AndroidBillingResult billingResult, IList<Purchase> purchases)
    {
        _onResponse(billingResult, purchases);
    }
}

#endregion
using BillingService.Models;
using Microsoft.Extensions.Logging;
using Windows.Services.Store;

namespace BillingService.Services;

public class BillingService : BaseBillingService
{
    private StoreContext? _storeContext;
    private string? _storeId;

    public BillingService(ILogger<BaseBillingService> logger) : base(logger)
    {
    }

    protected override Task<bool> InitializePlatformAsync()
    {
        try
        {
            // Get the default store context for the current user
            _storeContext = StoreContext.GetDefault();

            if (_storeContext == null)
            {
                _logger.LogError("Failed to get StoreContext - app may not be associated with Microsoft Store");
                return Task.FromResult(false);
            }

            // For testing purposes, log initialization success
            _logger.LogInformation("Windows Store context initialized successfully");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Windows billing service");
            return Task.FromResult(false);
        }
    }

    protected override async Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts)
    {
        try
        {
            if (_storeContext == null)
            {
                _logger.LogWarning("Store context not initialized, returning base products");
                return baseProducts;
            }

            // Batch query all products from Store for efficiency
            var productIds = baseProducts.Select(p => p.Id).ToArray();
            
            if (productIds.Length == 0)
            {
                _logger.LogWarning("No products to query");
                return baseProducts;
            }

            try
            {
                var storeProductsResult = await _storeContext.GetStoreProductsAsync(productIds);

                if (storeProductsResult.ExtendedError != null)
                {
                    _logger.LogWarning("Failed to retrieve products from Store: {Error}", storeProductsResult.ExtendedError.Message);
                    return baseProducts;
                }

                // Create a dictionary for quick lookup of store products
                var storeProductsDict = storeProductsResult.Products;
                var products = new List<Product>();

                foreach (var baseProduct in baseProducts)
                {
                    Product product;

                    // Check if product exists in Store
                    if (storeProductsDict.TryGetValue(baseProduct.Id, out var storeProduct))
                    {
                        // Create product with Store-retrieved information
                        product = new Product
                        {
                            Id = baseProduct.Id,
                            Name = storeProduct.Title ?? baseProduct.Name,
                            Description = storeProduct.Description ?? baseProduct.Description,
                            Price = storeProduct.Price?.FormattedPrice ?? baseProduct.Price,
                            PriceAmount = storeProduct.Price?.UnformattedPrice ?? baseProduct.PriceAmount,
                            ImageUrl = baseProduct.ImageUrl,
                            IsOwned = false // Will be set by base class
                        };

                        _logger.LogInformation("Retrieved product from Store: {ProductId}", baseProduct.Id);
                    }
                    else
                    {
                        // Product not found in Store, use base product
                        product = baseProduct;
                        _logger.LogWarning("Product not found in Store, using fallback: {ProductId}", baseProduct.Id);
                    }

                    products.Add(product);
                }

                _logger.LogInformation("Retrieved {Count} products from Windows Store", products.Count);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying products from Store, using fallback");
                return baseProducts;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get platform products");
            return baseProducts;
        }
    }

    protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
    {
        try
        {
            if (_storeContext == null)
            {
                return new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = "Store context not initialized. App must be associated with Microsoft Store."
                };
            }

            _logger.LogInformation("Initiating purchase for product: {ProductId}", productId);

            // Request purchase from Store
            var purchaseResult = await _storeContext.RequestPurchaseAsync(productId);

            if (purchaseResult.ExtendedError == null)
            {
                switch (purchaseResult.Status)
                {
                    case StorePurchaseStatus.Succeeded:
                        _logger.LogInformation("Purchase successful for product: {ProductId}", productId);
                        return new PurchaseResult
                        {
                            IsSuccess = true,
                            ProductId = productId,
                            ErrorMessage = string.Empty
                        };

                    case StorePurchaseStatus.AlreadyPurchased:
                        _logger.LogInformation("Product already owned: {ProductId}", productId);
                        return new PurchaseResult
                        {
                            IsSuccess = true,
                            ProductId = productId,
                            ErrorMessage = "Product already owned"
                        };

                    case StorePurchaseStatus.NotPurchased:
                        _logger.LogWarning("Purchase was not completed for product: {ProductId}", productId);
                        return new PurchaseResult
                        {
                            IsSuccess = false,
                            ProductId = productId,
                            ErrorMessage = "Purchase was not completed"
                        };

                    case StorePurchaseStatus.NetworkError:
                        _logger.LogError("Network error during purchase of product: {ProductId}", productId);
                        return new PurchaseResult
                        {
                            IsSuccess = false,
                            ProductId = productId,
                            ErrorMessage = "Network error - please check your internet connection"
                        };

                    case StorePurchaseStatus.ServerError:
                        _logger.LogError("Server error during purchase of product: {ProductId}", productId);
                        return new PurchaseResult
                        {
                            IsSuccess = false,
                            ProductId = productId,
                            ErrorMessage = "Server error - please try again later"
                        };

                    default:
                        _logger.LogWarning("Unknown purchase status for product: {ProductId}, Status: {Status}", productId, purchaseResult.Status);
                        return new PurchaseResult
                        {
                            IsSuccess = false,
                            ProductId = productId,
                            ErrorMessage = $"Unknown purchase status: {purchaseResult.Status}"
                        };
                }
            }
            else
            {
                _logger.LogError("Purchase error for product {ProductId}: {Error}", productId, purchaseResult.ExtendedError.Message);
                return new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = purchaseResult.ExtendedError.Message
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during purchase of product: {ProductId}", productId);
            return new PurchaseResult
            {
                IsSuccess = false,
                ProductId = productId,
                ErrorMessage = $"Exception: {ex.Message}"
            };
        }
    }

    protected override async Task<List<string>> GetPlatformPurchasedProductsAsync()
    {
        try
        {
            if (_storeContext == null)
            {
                _logger.LogWarning("Store context not initialized, returning empty list");
                return new List<string>();
            }

            var purchasedProducts = new List<string>();

            // Query user's licenses for the sample product IDs
            var sampleProductIds = _sampleProducts.Select(p => p.Id).ToArray();

            try
            {
                var userLicensesResult = await _storeContext.GetUserCollectionAsync(sampleProductIds);

                if (userLicensesResult.ExtendedError == null)
                {
                    foreach (var license in userLicensesResult.Licenses)
                    {
                        if (license.Value.IsActive)
                        {
                            purchasedProducts.Add(license.Key);
                            _logger.LogInformation("Found purchased product: {ProductId}", license.Key);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve user collection: {Error}", userLicensesResult.ExtendedError.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchased products from Store");
            }

            return purchasedProducts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get platform purchased products");
            return new List<string>();
        }
    }

    protected override async Task<bool> RestorePlatformPurchasesAsync()
    {
        try
        {
            if (_storeContext == null)
            {
                _logger.LogWarning("Store context not initialized, cannot restore purchases");
                return false;
            }

            _logger.LogInformation("Restoring purchases from Microsoft Store");

            // Get the user's purchased products
            var purchasedProducts = await GetPlatformPurchasedProductsAsync();

            if (purchasedProducts.Count > 0)
            {
                _logger.LogInformation("Successfully restored {Count} purchased products", purchasedProducts.Count);
                return true;
            }
            else
            {
                _logger.LogInformation("No previously purchased products found to restore");
                return true; // Still consider this a successful restoration (just no items to restore)
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore platform purchases");
            return false;
        }
    }
}

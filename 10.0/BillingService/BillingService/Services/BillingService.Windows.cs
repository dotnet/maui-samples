using BillingService.Models;
using Microsoft.Extensions.Logging;
using Windows.Services.Store;
using WinRT.Interop;

namespace BillingService.Services;

public class BillingService : BaseBillingService
{
    private StoreContext? _storeContext;

    public BillingService(ILogger<BaseBillingService> logger) : base(logger)
    {
    }

    protected override Task<bool> InitializePlatformAsync()
    {
        try
        {
            _storeContext = StoreContext.GetDefault();

            if (_storeContext == null)
            {
                _logger.LogError("Failed to get StoreContext - app may not be associated with Microsoft Store");
                return Task.FromResult(false);
            }

            // Initialize the StoreContext with the window handle (required for Desktop Bridge apps)
            // Per Microsoft documentation: https://aka.ms/storecontext-for-desktop
            InitializeWindowHandle();

            _isInitialized = true;
            _logger.LogInformation("Windows Store context initialized successfully");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Windows Store context");
            return Task.FromResult(false);
        }
    }

    private void InitializeWindowHandle()
    {
        var mainWindow = Application.Current?.Windows?.FirstOrDefault();
        if (mainWindow == null)
        {
            throw new InvalidOperationException("Could not retrieve main window - Store initialization requires a valid window");
        }

        var hwnd = WindowNative.GetWindowHandle(mainWindow.Handler?.PlatformView);
        if (hwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException("Window handle is invalid - Store initialization requires a valid window handle");
        }

        InitializeWithWindow.Initialize(_storeContext, hwnd);
        _logger.LogInformation("StoreContext initialized with window handle");
    }

    protected override async Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts)
    {
        try
        {
            _logger.LogInformation("Retrieving product information from Microsoft Store");
            
            if (_storeContext == null)
            {
                _logger.LogWarning("StoreContext is not initialized, using base product data");
                return baseProducts;
            }

            if (baseProducts.Count == 0)
            {
                return baseProducts;
            }

            var productIds = baseProducts.Select(p => p.Id).ToArray();
            var storeProductsResult = await _storeContext.GetStoreProductsAsync(
                productIds,
                new[] { "Consumable", "Durable", "UnknownType" }
            );

            if (storeProductsResult.ExtendedError != null)
            {
                _logger.LogWarning("Failed to retrieve products from Store: {Error}", storeProductsResult.ExtendedError.Message);
                return baseProducts;
            }

            var products = new List<Product>();
            var storeProductsDict = storeProductsResult.Products;

            foreach (var baseProduct in baseProducts)
            {
                Product product;

                if (storeProductsDict.TryGetValue(baseProduct.Id, out var storeProduct))
                {
                    product = new Product
                    {
                        Id = baseProduct.Id,
                        Name = storeProduct.Title ?? baseProduct.Name,
                        Description = storeProduct.Description ?? baseProduct.Description,
                        Price = storeProduct.Price?.FormattedPrice ?? baseProduct.Price,
                        PriceAmount = baseProduct.PriceAmount,
                        ImageUrl = baseProduct.ImageUrl,
                        IsOwned = false
                    };

                    _logger.LogInformation("Retrieved product from Store: {ProductId}", baseProduct.Id);
                }
                else
                {
                    product = baseProduct;
                    _logger.LogWarning("Product not found in Store, using local data: {ProductId}", baseProduct.Id);
                }

                products.Add(product);
            }

            _logger.LogInformation("Successfully retrieved {Count} products from Store", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products from Store");
            return baseProducts;
        }
    }

    protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
    {
        try
        {
            _logger.LogInformation("Initiating purchase for product: {ProductId}", productId);

            if (_storeContext == null)
            {
                _logger.LogError("StoreContext is not initialized");
                return new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = "Store context not initialized. App must be associated with Microsoft Store."
                };
            }

            var purchaseResult = await _storeContext.RequestPurchaseAsync(productId);

            if (purchaseResult.ExtendedError != null)
            {
                _logger.LogError("Purchase failed with error: {ErrorMessage}", 
                    purchaseResult.ExtendedError.Message);
                return new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = purchaseResult.ExtendedError.Message
                };
            }

            switch (purchaseResult.Status)
            {
                case StorePurchaseStatus.Succeeded:
                    _logger.LogInformation("Purchase succeeded for product: {ProductId}", productId);
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
                    _logger.LogInformation("Purchase was not completed for product: {ProductId}", productId);
                    return new PurchaseResult
                    {
                        IsSuccess = false,
                        ProductId = productId,
                        ErrorMessage = "Purchase was not completed"
                    };

                case StorePurchaseStatus.NetworkError:
                    _logger.LogError("Network error during purchase of: {ProductId}", productId);
                    return new PurchaseResult
                    {
                        IsSuccess = false,
                        ProductId = productId,
                        ErrorMessage = "Network error - please check your internet connection"
                    };

                case StorePurchaseStatus.ServerError:
                    _logger.LogError("Server error during purchase of: {ProductId}", productId);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during purchase for product {ProductId}", productId);
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
                _logger.LogWarning("StoreContext is not initialized, cannot retrieve purchased products");
                return new List<string>();
            }

            var sampleProductIds = _sampleProducts.Select(p => p.Id).ToArray();
            var userCollectionResult = await _storeContext.GetUserCollectionAsync(sampleProductIds);

            if (userCollectionResult.ExtendedError != null)
            {
                _logger.LogWarning("Failed to retrieve user collection: {ErrorMessage}", userCollectionResult.ExtendedError.Message);
                return new List<string>();
            }

            var purchasedProducts = new List<string>();
            foreach (var product in userCollectionResult.Products.Values)
            {
                purchasedProducts.Add(product.StoreId);
            }

            _logger.LogInformation("Retrieved {Count} purchased products from Store", purchasedProducts.Count);
            return purchasedProducts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchased products from Store");
            return new List<string>();
        }
    }

    protected override async Task<bool> RestorePlatformPurchasesAsync()
    {
        try
        {
            if (_storeContext == null)
            {
                _logger.LogWarning("StoreContext is not initialized, cannot restore purchases");
                return false;
            }

            var purchasedProducts = await GetPlatformPurchasedProductsAsync();
            _logger.LogInformation("Restore purchases completed - restored {Count} products", purchasedProducts.Count);

            return true; // Return true even if no products (successful operation)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring purchases");
            return false;
        }
    }
}

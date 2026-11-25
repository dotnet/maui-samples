using BillingService.Models;
using BillingService.Services;
using Microsoft.Extensions.Logging;

namespace BillingService.Services;

public abstract class BaseBillingService : IBillingService
{
    protected readonly ILogger<BaseBillingService> _logger;
    protected bool _isInitialized;

    // Sample product definitions - shared across all platforms
    protected readonly List<Product> _sampleProducts = new()
    {
        new Product { Id = "Team_license", Name = "Team License", Description = "Team licenses offer the best value to get started", Price = "$300.99", PriceAmount = 400.99m, ImageUrl = "Team.png" },
        new Product { Id = "Global_license", Name = "Global License", Description = "Get Our Entire Product Line for Free", Price = "$600.99", PriceAmount = 700.99m, ImageUrl = "Global_license.png" },
        new Product { Id = "Unlimited_license", Name = "Unlimited License", Description = "Cover everyone for one low, annual fee", Price = "$700.99", PriceAmount = 600.99m, ImageUrl = "no_ads.png" }
    };

    // For demo purposes, simulate 2 owned items initially
    protected readonly HashSet<string> _ownedProducts = new() { "Team_license" };

    public bool IsInitialized => _isInitialized;

    protected BaseBillingService(ILogger<BaseBillingService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> InitializeAsync()
    {
        if (_isInitialized)
            return true;

        try
        {
            var result = await InitializePlatformAsync();
            _isInitialized = result;
            _logger.LogInformation("Billing service initialized: {Success}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize billing service");
            return false;
        }
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }

        try
        {
            // Get platform-specific product details
            var products = await GetPlatformProductsAsync(_sampleProducts);

            // Mark owned products
            foreach (var product in products)
            {
                product.IsOwned = _ownedProducts.Contains(product.Id);
            }

            _logger.LogInformation("Retrieved {Count} products", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products");
            return _sampleProducts.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                PriceAmount = p.PriceAmount,
                ImageUrl = p.ImageUrl,
                IsOwned = _ownedProducts.Contains(p.Id)
            }).ToList();
        }
    }

    public async Task<PurchaseResult> PurchaseAsync(string productId)
    {
        _logger.LogInformation("Attempting to purchase product: {ProductId}", productId);

        try
        {
            var result = await PurchasePlatformProductAsync(productId);

            if (result.IsSuccess)
            {
                _ownedProducts.Add(productId);
                _logger.LogInformation("Purchase successful for product: {ProductId}", productId);
            }
            else
            {
                _logger.LogWarning("Purchase failed for product: {ProductId}, Error: {Error}", productId, result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during purchase: {ProductId}", productId);
            return new PurchaseResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                ProductId = productId
            };
        }
    }

    public async Task<List<string>> GetPurchasedProductsAsync()
    {
        try
        {
            var platformOwned = await GetPlatformPurchasedProductsAsync();

            // Merge with local owned products
            foreach (var product in platformOwned)
            {
                _ownedProducts.Add(product);
            }

            return _ownedProducts.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get purchased products");
            return _ownedProducts.ToList();
        }
    }
    public async Task<bool> RestorePurchasesAsync()
    {
        try
        {
            var success = await RestorePlatformPurchasesAsync();
            _logger.LogInformation("Purchases restored: {Success}", success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore purchases");
            return false;
        }
    }

    // Abstract methods to be implemented by platform-specific classes
    protected abstract Task<bool> InitializePlatformAsync();
    protected abstract Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts);
    protected abstract Task<PurchaseResult> PurchasePlatformProductAsync(string productId);
    protected abstract Task<List<string>> GetPlatformPurchasedProductsAsync();
    protected abstract Task<bool> RestorePlatformPurchasesAsync();
}

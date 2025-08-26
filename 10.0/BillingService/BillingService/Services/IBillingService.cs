using BillingService.Models;

namespace BillingService.Services;

public interface IBillingService
{
    Task<bool> InitializeAsync();
    Task<List<Product>> GetProductsAsync();
    Task<PurchaseResult> PurchaseAsync(string productId);
    Task<List<string>> GetPurchasedProductsAsync();
    Task<bool> RestorePurchasesAsync();
    bool IsInitialized { get; }
}

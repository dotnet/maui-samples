using BillingService.Models;
using Microsoft.Extensions.Logging;
using Foundation;
using StoreKit;

namespace BillingService.Services;

#pragma warning disable CA1422 // Validate platform compatibility
public partial class BillingService : BaseBillingService
{
    private PaymentTransactionObserver? _paymentObserver;
    private TaskCompletionSource<bool>? _purchaseTaskCompletionSource;

    public BillingService(ILogger<BaseBillingService> logger) : base(logger) { }

    protected override Task<bool> InitializePlatformAsync()
    {
        try
        {
            if (!SKPaymentQueue.CanMakePayments)
            {
                _logger.LogError("In-app purchases are disabled on this device");
                return Task.FromResult(false);
            }
            if (_paymentObserver != null)
                return Task.FromResult(true);
            _paymentObserver = new PaymentTransactionObserver(this);
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(_paymentObserver);
            _logger.LogInformation("Apple billing service initialized successfully");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Apple billing service");
            return Task.FromResult(false);
        }
    }

    protected override Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts)
    {
        try
        {
            var products = baseProducts.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                PriceAmount = p.PriceAmount,
                ImageUrl = p.ImageUrl,
                IsOwned = _ownedProducts.Contains(p.Id)
            }).ToList();
            _logger.LogInformation("Retrieved {Count} products from Apple", products.Count);
            return Task.FromResult(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting platform products");
            foreach (var product in baseProducts)
                product.IsOwned = _ownedProducts.Contains(product.Id);
            return Task.FromResult(baseProducts);
        }
    }

    protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
    {
        try
        {
            var productName = _sampleProducts.FirstOrDefault(p => p.Id == productId)?.Name ?? productId;
            if (!SKPaymentQueue.CanMakePayments)
            {
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "In-app purchases are disabled on this device" };
            }

            var productIds = new[] { productId };
            var productIdentifiers = NSSet.MakeNSObjectSet(productIds.Select(id => new NSString(id)).ToArray());
            var requestDelegate = new ProductsRequestDelegate();
            var productsRequest = new SKProductsRequest(productIdentifiers) { Delegate = requestDelegate };
            productsRequest.Start();
            var products = await requestDelegate.GetProductsAsync();

            if (products == null || !products.Any())
            {
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = $"{productName} not found in App Store" };
            }

            var product = products.First();
            if (product == null)
            {
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = $"Product details unavailable for {productName}" };
            }

            _purchaseTaskCompletionSource = new TaskCompletionSource<bool>();
            var payment = SKPayment.CreateFrom(product);
            SKPaymentQueue.DefaultQueue.AddPayment(payment);
            return new PurchaseResult { IsSuccess = true, ProductId = productId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during purchase flow for product {ProductId}", productId);
            return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = $"Purchase failed: {ex.Message}" };
        }
    }

    protected override Task<List<string>> GetPlatformPurchasedProductsAsync()
    {
        var purchasedProducts = _ownedProducts.ToList();
        _logger.LogInformation("Retrieved {Count} purchased products", purchasedProducts.Count);
        return Task.FromResult(purchasedProducts);
    }

    protected override async Task<bool> RestorePlatformPurchasesAsync()
    {
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            var validProductIds = _sampleProducts.Select(p => p.Id).ToHashSet();

            EventHandler? completedHandler = null;
            EventHandler<NSError>? errorHandler = null;

            completedHandler = (sender, e) =>
            {
                var transactions = SKPaymentQueue.DefaultQueue.Transactions;
                if (transactions != null)
                {
                    foreach (var transaction in transactions)
                    {
                        var pid = transaction.Payment?.ProductIdentifier;
                        if (!string.IsNullOrEmpty(pid) && validProductIds.Contains(pid))
                        {
                            if (transaction.TransactionState == SKPaymentTransactionState.Purchased ||
                                transaction.TransactionState == SKPaymentTransactionState.Restored)
                            {
                                _ownedProducts.Add(pid);
                            }
                            SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                        }
                    }
                }
                tcs.TrySetResult(true);
            };

            errorHandler = (sender, error) =>
            {
                if (error?.Code == 2)
                    _logger.LogInformation("User cancelled restore purchases");
                else
                    _logger.LogWarning("Restore failed with error code: {Code}", error?.Code);
                tcs.TrySetResult(false);
            };

            if (_paymentObserver == null)
            {
                _logger.LogError("Payment observer is not initialized");
                return false;
            }

            _paymentObserver.RestoreCompletedTransactionsFinishedEvent += completedHandler;
            _paymentObserver.RestoreCompletedTransactionsFailedEvent += errorHandler;

            SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
            var result = await tcs.Task;
            _logger.LogInformation("Restored purchases: {Success}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring purchases");
            return false;
        }
    }

    internal void OnTransactionUpdated(SKPaymentTransaction transaction)
    {
        var pid = transaction.Payment?.ProductIdentifier;
        switch (transaction.TransactionState)
        {
            case SKPaymentTransactionState.Purchased:
                if (!string.IsNullOrEmpty(pid)) _ownedProducts.Add(pid);
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                _purchaseTaskCompletionSource?.TrySetResult(true);
                break;
            case SKPaymentTransactionState.Failed:
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                _purchaseTaskCompletionSource?.TrySetResult(false);
                break;
            case SKPaymentTransactionState.Restored:
                if (!string.IsNullOrEmpty(pid)) _ownedProducts.Add(pid);
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                break;
        }
    }

    ~BillingService()
    {
        if (_paymentObserver != null)
        {
            SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(_paymentObserver);
            _paymentObserver.Dispose();
        }
    }
}

internal class ProductsRequestDelegate : NSObject, ISKProductsRequestDelegate, ISKRequestDelegate
{
    private readonly TaskCompletionSource<List<SKProduct>> _tcs = new();
    public Task<List<SKProduct>> GetProductsAsync() => _tcs.Task;
    [Export("productsRequest:didReceiveResponse:")]
    public void ReceivedResponse(SKProductsRequest request, SKProductsResponse response) =>
        _tcs.TrySetResult(response.Products?.ToList() ?? new List<SKProduct>());
    [Export("request:didFailWithError:")]
    public void RequestFailed(SKRequest request, NSError error) =>
        _tcs.TrySetException(new Exception(error.LocalizedDescription));
}

internal class PaymentTransactionObserver : SKPaymentTransactionObserver
{
    private readonly BillingService _billingService;
    public event EventHandler? RestoreCompletedTransactionsFinishedEvent;
    public event EventHandler<NSError>? RestoreCompletedTransactionsFailedEvent;
    public PaymentTransactionObserver(BillingService billingService) => _billingService = billingService;
    public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
    {
        foreach (var t in transactions) _billingService.OnTransactionUpdated(t);
    }
    public override void RestoreCompletedTransactionsFinished(SKPaymentQueue queue) =>
        RestoreCompletedTransactionsFinishedEvent?.Invoke(this, EventArgs.Empty);
    public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error) =>
        RestoreCompletedTransactionsFailedEvent?.Invoke(this, error);
}
#pragma warning restore CA1422 // Validate platform compatibility

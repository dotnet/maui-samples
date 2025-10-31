using BillingService.Models;
using Microsoft.Extensions.Logging;
using Foundation;
using StoreKit;

namespace BillingService.Services;

// Suppress StoreKit 1 deprecation warnings
// Note: StoreKit 1 APIs are deprecated in iOS 18.0+ but remain fully functional
// StoreKit 2 (modern replacement) lacks comprehensive C# bindings in .NET MAUI
// This implementation is standard practice for .NET MAUI apps as of 2025
#pragma warning disable CA1422 // Validate platform compatibility
public class BillingService : BaseBillingService
{
    private PaymentTransactionObserver? _paymentObserver;
    private TaskCompletionSource<bool>? _purchaseTaskCompletionSource;

    public BillingService(ILogger<BaseBillingService> logger) : base(logger)
    {
    }

    protected override Task<bool> InitializePlatformAsync()
    {
        try
        {
            // Check if payments are available
            if (!SKPaymentQueue.CanMakePayments)
            {
                _logger.LogError("In-app purchases are disabled on this device");
                return Task.FromResult(false);
            }

            // Check if observer is initialized
            if (_paymentObserver != null)
            {
                _logger.LogWarning("Payment observer already initialized");
                return Task.FromResult(true);
            }

            // Initialize and register transaction observer
            _paymentObserver = new PaymentTransactionObserver(this);
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(_paymentObserver);
            
            _logger.LogInformation("iOS billing service initialized successfully");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize iOS billing service");
            return Task.FromResult(false);
        }
    }

    protected override Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts)
    {
        try
        {
            // For iOS, we return base products with ownership status updated
            // In a real app, you would query App Store product details here
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

            _logger.LogInformation("Retrieved {Count} products from iOS", products.Count);
            return Task.FromResult(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting platform products");
            
            // Return base products with ownership status on error
            foreach (var product in baseProducts)
            {
                product.IsOwned = _ownedProducts.Contains(product.Id);
            }
            
            return Task.FromResult(baseProducts);
        }
    }

    protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
    {
        try
        {
            // Get product name for better error messages
            var productName = _sampleProducts.FirstOrDefault(p => p.Id == productId)?.Name ?? productId;

            // Check if payments are available
            if (!SKPaymentQueue.CanMakePayments)
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = "In-app purchases are disabled on this device"
                });
            }

            // Query product details from App Store
            var productIds = new[] { productId };
            var productIdentifiers = NSSet.MakeNSObjectSet(productIds.Select(id => new NSString(id)).ToArray());
            var requestDelegate = new ProductsRequestDelegate();
            var productsRequest = new SKProductsRequest(productIdentifiers) { Delegate = requestDelegate };

            productsRequest.Start();
            var products = await requestDelegate.GetProductsAsync();

            // Validate product exists
            if (products == null || !products.Any())
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = $"{productName} not found in App Store"
                });
            }

            var product = products.First();
            if (product == null)
            {
                return await Task.FromResult(new PurchaseResult
                {
                    IsSuccess = false,
                    ProductId = productId,
                    ErrorMessage = $"Product details unavailable for {productName}"
                });
            }

            // Create payment and launch purchase flow
            _purchaseTaskCompletionSource = new TaskCompletionSource<bool>();
            var payment = SKPayment.CreateFrom(product);
            SKPaymentQueue.DefaultQueue.AddPayment(payment);

            // Note: The actual purchase result comes from OnTransactionUpdated callback
            // This just initiates the flow, similar to Android's LaunchBillingFlow
            return await Task.FromResult(new PurchaseResult
            {
                IsSuccess = true,
                ProductId = productId,
                ErrorMessage = ""
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during purchase flow for product {ProductId}", productId);
            return await Task.FromResult(new PurchaseResult
            {
                IsSuccess = false,
                ProductId = productId,
                ErrorMessage = $"Purchase failed: {ex.Message}"
            });
        }
    }

    protected override Task<List<string>> GetPlatformPurchasedProductsAsync()
    {
        // For iOS, return the locally tracked owned products
        // In a real app, you might query the receipt or server
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
                        var productId = transaction.Payment?.ProductIdentifier;
                        if (!string.IsNullOrEmpty(productId) && validProductIds.Contains(productId))
                        {
                            if (transaction.TransactionState == SKPaymentTransactionState.Purchased ||
                                transaction.TransactionState == SKPaymentTransactionState.Restored)
                            {
                                _ownedProducts.Add(productId);
                            }
                            SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                        }
                    }
                }
                tcs.TrySetResult(true);
            };

            errorHandler = (sender, error) =>
            {
                // Error code 2 = user cancelled (don't show error message)
                // Other errors = actual error (will be logged)
                if (error?.Code == 2)
                {
                    _logger.LogInformation("User cancelled restore purchases");
                }
                else
                {
                    _logger.LogWarning("Restore failed with error code: {Code}", error?.Code);
                }
                tcs.TrySetResult(false);
            };

            try
            {
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
            finally
            {
                if (_paymentObserver != null)
                {
                    if (completedHandler != null)
                        _paymentObserver.RestoreCompletedTransactionsFinishedEvent -= completedHandler;
                    if (errorHandler != null)
                        _paymentObserver.RestoreCompletedTransactionsFailedEvent -= errorHandler;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring purchases");
            return false;
        }
    }

    internal void OnTransactionUpdated(SKPaymentTransaction transaction)
    {
        var productId = transaction.Payment?.ProductIdentifier;

        switch (transaction.TransactionState)
        {
            case SKPaymentTransactionState.Purchased:
                if (!string.IsNullOrEmpty(productId))
                    _ownedProducts.Add(productId);
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                _purchaseTaskCompletionSource?.TrySetResult(true);
                break;

            case SKPaymentTransactionState.Failed:
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                _purchaseTaskCompletionSource?.TrySetResult(false);
                break;

            case SKPaymentTransactionState.Restored:
                if (!string.IsNullOrEmpty(productId))
                    _ownedProducts.Add(productId);
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
    private readonly TaskCompletionSource<List<SKProduct>> _taskCompletionSource = new();

    public Task<List<SKProduct>> GetProductsAsync() => _taskCompletionSource.Task;

    [Export("productsRequest:didReceiveResponse:")]
    public void ReceivedResponse(SKProductsRequest request, SKProductsResponse response)
    {
        _taskCompletionSource.TrySetResult(response.Products?.ToList() ?? new List<SKProduct>());
    }

    [Export("request:didFailWithError:")]
    public void RequestFailed(SKRequest request, NSError error)
    {
        _taskCompletionSource.TrySetException(new Exception(error.LocalizedDescription));
    }
}

internal class PaymentTransactionObserver : SKPaymentTransactionObserver
{
    private readonly BillingService _billingService;

    public event EventHandler? RestoreCompletedTransactionsFinishedEvent;
    public event EventHandler<NSError>? RestoreCompletedTransactionsFailedEvent;

    public PaymentTransactionObserver(BillingService billingService)
    {
        _billingService = billingService;
    }

    public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
    {
        foreach (var transaction in transactions)
            _billingService.OnTransactionUpdated(transaction);
    }

    public override void RestoreCompletedTransactionsFinished(SKPaymentQueue queue)
    {
        RestoreCompletedTransactionsFinishedEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error)
    {
        RestoreCompletedTransactionsFailedEvent?.Invoke(this, error);
    }
}
#pragma warning restore CA1422 // Validate platform compatibility
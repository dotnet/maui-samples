using System.Collections.ObjectModel;
using System.Windows.Input;
using BillingService.Models;
using BillingService.Services;
using Microsoft.Extensions.Logging;

namespace BillingService.ViewModels;

public class ProductsViewModel : BaseViewModel
{
    private bool isLoading;
    private readonly IBillingService _billingService;
    private readonly ILogger<ProductsViewModel> _logger;

    public ObservableCollection<Product> Products { get; } = new();

    public ICommand RestorePurchasesCommand { get; }
    public ICommand LoadProductsCommand { get; }
    public ICommand PurchaseCommand { get; }

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public ProductsViewModel(IBillingService billingService, ILogger<ProductsViewModel> logger)
    {
        _billingService = billingService;
        _logger = logger;
        Title = "Subscriptions & Purchases";

        LoadProductsCommand = new Command(async () => await LoadProductsAsync());
        PurchaseCommand = new Command<Product>(async (product) => await PurchaseProductAsync(product));
        RestorePurchasesCommand = new Command(async () => await RestorePurchasesAsync());
    }

    public async Task InitializeAsync()
    {
        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            if (!_billingService.IsInitialized)
            {
                IsLoading = true;
                await Task.Delay(2000);
                var initialized = await _billingService.InitializeAsync();

                if (!initialized)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Failed to initialize billing service", "OK");
                    return;
                }
            }

            var products = await _billingService.GetProductsAsync();

            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }

            _logger.LogInformation("Loaded {Count} products", products.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Failed to load products", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task PurchaseProductAsync(Product product)
    {
        if (product == null || product.IsOwned)
            return;

        try
        {
            var confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Confirm Purchase",
                $"Purchase {product.Name} for {product.Price}?",
                "Yes",
                "No");

            if (!confirm)
                return;

            // Show loading indicator during purchase
            IsLoading = true;

            var result = await _billingService.PurchaseAsync(product.Id);

            if (result.IsSuccess)
            {
                product.IsOwned = true;
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", $"Successfully purchased {product.Name}!", "OK");
                _logger.LogInformation("Purchase successful: {ProductId}", product.Id);
            }
            else
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Purchase Failed", result.ErrorMessage, "OK");
                _logger.LogWarning("Purchase failed: {ProductId}, Error: {Error}", product.Id, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during purchase: {ProductId}", product.Id);
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "An error occurred during purchase", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RestorePurchasesAsync()
    {
        try
        {
            IsLoading = true;

            var success = await _billingService.RestorePurchasesAsync();

            if (success)
            {
                await (Application.Current?.Windows[0].Page?.DisplayAlertAsync("Success", "Purchases restored successfully!", "OK") ?? Task.CompletedTask);
                _logger.LogInformation("Purchases restored successfully");
            }
            else
            {
                await (Application.Current?.Windows[0].Page?.DisplayAlertAsync("Error", "Failed to restore purchases", "OK") ?? Task.CompletedTask);
                _logger.LogWarning("Failed to restore purchases");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring purchases");
            await (Application.Current?.Windows[0].Page?.DisplayAlertAsync("Error", "An error occurred while restoring purchases", "OK") ?? Task.CompletedTask);
        }
        finally
        {
            IsLoading = false;
        }
    }
}

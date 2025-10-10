using BillingService.ViewModels;

namespace BillingService.Views;

public partial class ProductsPage : ContentPage
{
    private readonly ProductsViewModel _viewModel;

	public ProductsPage(ProductsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
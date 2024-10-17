namespace PointOfSale.Pages.Handheld;

[QueryProperty("Order","Order")]
public partial class ReceiptViewModel : ObservableObject
{
    [ObservableProperty]
    Order order;

    [RelayCommand]
    async Task Done()
    {
        await Shell.Current.GoToAsync("///orders");
    }
}
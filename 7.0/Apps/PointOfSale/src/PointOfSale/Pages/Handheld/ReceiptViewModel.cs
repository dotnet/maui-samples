namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
[QueryProperty("Order","Order")]
public partial class ReceiptViewModel
{
    [ObservableProperty]
    Order order;

    [RelayCommand]
    async void Done()
    {
        await Shell.Current.GoToAsync("///orders");
    }
}
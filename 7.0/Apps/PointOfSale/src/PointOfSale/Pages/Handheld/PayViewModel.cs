namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
[QueryProperty("Order","Order")]
public partial class PayViewModel
{
    [ObservableProperty]
    Order order;
    
    [RelayCommand]
    async void Pay()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", order }
        };
        await Shell.Current.GoToAsync($"{nameof(SignaturePage)}", navigationParameter);
    }
}
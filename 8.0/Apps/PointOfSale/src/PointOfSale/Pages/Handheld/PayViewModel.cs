namespace PointOfSale.Pages.Handheld;

[QueryProperty("Order","Order")]
public partial class PayViewModel : ObservableObject
{
    [ObservableProperty]
    Order order;
    
    [RelayCommand]
    async Task Pay()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", Order }
        };
        await Shell.Current.GoToAsync($"{nameof(SignaturePage)}", navigationParameter);
    }
}
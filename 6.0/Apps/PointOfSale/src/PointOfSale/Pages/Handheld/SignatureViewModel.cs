namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
[QueryProperty("Order","Order")]
public partial class SignatureViewModel
{
    [ObservableProperty]
    Order order;
    
    [RelayCommand]
    async Task Done()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", order }
        };
        await Shell.Current.GoToAsync($"{nameof(ReceiptPage)}", navigationParameter);
    }

    [RelayCommand]
    void Clear()
    {
        // msg the signature pad
    }
}
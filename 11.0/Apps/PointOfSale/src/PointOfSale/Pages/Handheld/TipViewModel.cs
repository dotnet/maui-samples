namespace PointOfSale.Pages.Handheld;

[QueryProperty("Order","Order")]
public partial class TipViewModel : ObservableObject
{
    [ObservableProperty]
    Order order;

    [ObservableProperty]
    double tip;

    partial void OnTipChanged(double value)
    {
        Order.Tip = value;
        OnPropertyChanged(nameof(Order));
    }

    [RelayCommand]
    async Task Continue()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", Order }
        };
        await Shell.Current.GoToAsync($"{nameof(PayPage)}", navigationParameter);
    }

    [RelayCommand]
    async Task Add()
    {
        await Shell.Current.GoToAsync($"{nameof(ScanPage)}");
    }
}   
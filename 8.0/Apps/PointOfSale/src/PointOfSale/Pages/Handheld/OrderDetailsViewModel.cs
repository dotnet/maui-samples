namespace PointOfSale.Pages.Handheld;

[QueryProperty("Order","Order")]
[QueryProperty("Added", "Added")]
public partial class OrderDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    Order order;

    [ObservableProperty]
    Item added;

    [RelayCommand]
    async Task Pay()
    {
        try
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Order", Order }
            };
            await Shell.Current.GoToAsync($"{nameof(TipPage)}", navigationParameter);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
public partial class OrdersViewModel
{
    [ObservableProperty]
    private ObservableCollection<Order> _orders;

    [ObservableProperty]
    private string pageCurrentState = "Loading";

    [RelayCommand]
    public async Task LogOut()
    {
        try
        {
            await PointOfSale.MSALClient.PCAWrapper.Instance.SignOutAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Oops", "Unable to complete the logout", "Okay");
        }

        await Shell.Current.GoToAsync("//login");
    }

    public OrdersViewModel()
    {
        _orders = new ObservableCollection<Order>(AppData.Orders);

        DelayedLoad().ConfigureAwait(false);
    }

    private async Task DelayedLoad()
    {
        await Task.Delay(4000);
        PageCurrentState = "Loaded";
    }
}
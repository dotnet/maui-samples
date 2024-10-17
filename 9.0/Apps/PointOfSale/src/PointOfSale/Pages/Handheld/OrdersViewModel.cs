using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Reflection;


namespace PointOfSale.Pages.Handheld;

public partial class OrdersViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Order> _orders;

    [ObservableProperty]
    private string displayName;

    [ObservableProperty]
    private string displayEmail;

    [ObservableProperty]
    private ImageSource profilePhoto;

    [ObservableProperty]
    private string pageCurrentState = "Loading";

    [RelayCommand]
    public async Task LogOut()
    {
        var result = await App.Current.MainPage.DisplayAlert("", "Do you want to logout?", "Yes", "Ooops, no");
        if (!result)
            return;

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
using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Reflection;


namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
public partial class OrdersViewModel
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

        try
        {
            await PublicClientSingleton.Instance.SignOutAsync();
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
        LoadUserProfile();
        DelayedLoad().ConfigureAwait(false);
    }

    private void LoadUserProfile()
    {
        _ = GetUserInformationAsync();
    }

    private async Task GetUserInformationAsync()
    {
        try
        {
            var user = await PublicClientSingleton.Instance.MSGraphHelper.GetMeAsync();
            var photoStream = await PublicClientSingleton.Instance.MSGraphHelper.GetMyPhotoAsync();
            var userPhoto = ImageSource.FromStream(()=> { return photoStream; });
            
            if (userPhoto is not null)
            {
                ProfilePhoto = userPhoto;
            }

            DisplayName = user.DisplayName;
            DisplayEmail = user.Mail;
        }
        catch (MsalUiRequiredException)
        {
            await PublicClientSingleton.Instance.SignOutAsync();
            //await PublicClientWrapper.Instance.SignOutAsync().ContinueWith((t) =>
            //{
            //    return Task.CompletedTask;
            //});

            await Shell.Current.GoToAsync("//login");
        }
    }


    private async Task DelayedLoad()
    {
        await Task.Delay(4000);
        PageCurrentState = "Loaded";
    }
}
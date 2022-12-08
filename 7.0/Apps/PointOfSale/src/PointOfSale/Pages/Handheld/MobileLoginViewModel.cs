using MAUI.MSALClient;

namespace PointOfSale.Pages.Handheld;

[INotifyPropertyChanged]
public partial class MobileLoginViewModel
{
    [RelayCommand]
    async Task Login()
    {
        PublicClientSingleton.Instance.UseEmbedded = false; // this.useEmbedded.IsChecked;

        try
        {
            await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
        }
        catch (MsalClientException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
        {
            await ShowMessage("Login failed", "User cancelled sign in.");
            return;
        }

        await Shell.Current.GoToAsync("//orders");
    }

    // display the message
    private async Task ShowMessage(string title, string message)
    {
        _ = App.Current.MainPage.Dispatcher.Dispatch(async () =>
        {
            await App.Current.MainPage.DisplayAlert(title, message, "OK").ConfigureAwait(false);
        });
    }
}
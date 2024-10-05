
namespace PointOfSale.Pages.Handheld;

public partial class MobileLoginViewModel : ObservableObject
{
    [RelayCommand]
    async Task Login()
    {
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
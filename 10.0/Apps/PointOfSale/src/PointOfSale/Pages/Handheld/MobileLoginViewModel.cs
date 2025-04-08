
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
        _ = App.Current.Windows[0].Page.Dispatcher.Dispatch(async () =>
        {
            await App.Current.Windows[0].Page.DisplayAlert(title, message, "OK").ConfigureAwait(false);
        });
    }
}

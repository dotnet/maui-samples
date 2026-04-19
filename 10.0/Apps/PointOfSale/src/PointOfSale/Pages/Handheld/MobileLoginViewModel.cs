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
        // Run on UI thread and call the Page.DisplayAlertAsync method
        _ = App.Current.Windows[0].Page.Dispatcher.Dispatch(async () =>
        {
            await App.Current.Windows[0].Page.DisplayAlertAsync(title, message, "OK").ConfigureAwait(false);
        });
    }
}

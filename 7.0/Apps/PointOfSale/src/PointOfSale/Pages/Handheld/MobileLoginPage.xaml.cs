using MAUI.MSALClient;

namespace PointOfSale.Pages.Handheld;

public partial class MobileLoginPage : ContentPage
{
	public MobileLoginPage()
	{
		InitializeComponent();
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);


        IAccount cachedUserAccount = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache()).Result;

        _ = Dispatcher.DispatchAsync(async () =>
        {
            if (cachedUserAccount == null)
            {
                //SignInButton.IsEnabled = true;
            }
            else
            {
                await Shell.Current.GoToAsync("//orders");
            }
        });
    }
}

using PointOfSale.MSALClient;

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

        
        // if logged in, move on
        AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenSilentAsync(PCAWrapper.Scopes).ConfigureAwait(false);
        //var token = await MSALClient.PCAWrapper.Instance.GetAuthenticationToken(MSALClient.PCAWrapper.Scopes).ConfigureAwait(false);
        if (result != null)
        {
            await Shell.Current.GoToAsync("//orders");
        }
    }
}

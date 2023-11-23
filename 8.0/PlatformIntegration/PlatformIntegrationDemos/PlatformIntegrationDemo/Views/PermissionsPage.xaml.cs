using CommunityToolkit.Mvvm.Messaging;

namespace PlatformIntegrationDemo.Views;

public partial class PermissionsPage : BasePage
{
	public PermissionsPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        WeakReferenceMessenger.Default.Register<Exception, string>(
            this,
            nameof(PermissionException),
            async (p, ex) => await DisplayAlert("Permission Error", ex.Message, "OK"));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        WeakReferenceMessenger.Default.Unregister<Exception, string>(this, nameof(PermissionException));
    }
}

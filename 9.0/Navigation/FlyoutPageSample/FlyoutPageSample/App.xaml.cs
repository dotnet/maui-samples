namespace FlyoutPageSample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppFlyout();
	}
}

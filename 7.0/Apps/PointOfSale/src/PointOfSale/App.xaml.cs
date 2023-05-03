namespace PointOfSale;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

        App.Current.UserAppTheme = AppTheme.Dark;

        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            MainPage = new AppShellMobile();
        }
        else
        {
            MainPage = new AppShell();
        }
	}
}
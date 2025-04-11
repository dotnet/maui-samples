namespace PointOfSale;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        App.Current.UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            return new Window(new AppShellMobile());
        }
        else
        {
            return new Window(new AppShell());
        }
    }
}

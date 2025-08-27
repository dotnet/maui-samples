namespace LoginNavigation;

public partial class App : Application
{
    public static bool IsUserLoggedIn { get; set; }

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (!IsUserLoggedIn) 
        {
            return new Window(new NavigationPage(new LoginPage()));
        } 
        else 
        {
            return new Window(new NavigationPage(new MainPage()));
        }
    }
}
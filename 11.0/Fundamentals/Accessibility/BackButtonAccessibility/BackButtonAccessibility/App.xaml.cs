namespace BackButtonAccessibility;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()))
        {
            Title = "Back Button Accessibility - .NET MAUI 11"
        };
    }
}

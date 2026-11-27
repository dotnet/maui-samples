namespace Material3HandlerCustomization;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage())
        {
            Title = "Material 3 Handler Customization - .NET MAUI 11"
        };
    }
}

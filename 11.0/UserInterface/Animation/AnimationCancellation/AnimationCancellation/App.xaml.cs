namespace AnimationCancellation;

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
            Title = "Animation Cancellation - .NET MAUI 11"
        };
    }
}

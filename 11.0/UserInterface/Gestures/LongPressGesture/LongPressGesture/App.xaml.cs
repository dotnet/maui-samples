namespace LongPressGesture;

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
            Title = "Long Press Gesture Demo - .NET MAUI"
        };
    }
}

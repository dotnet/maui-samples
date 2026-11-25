namespace PanGesture;

public partial class App : Application
{
    public static double ScreenWidth;
    public static double ScreenHeight;

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new HomePage());
    }
}
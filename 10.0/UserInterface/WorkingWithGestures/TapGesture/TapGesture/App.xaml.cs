namespace TapGesture;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var tabs = new TabbedPage
        {
            Title = "Tap Gesture Demo"
        };

        // demonstrates an Image tap (and changing the image)
        tabs.Children.Add(new TapInsideImage
        {
            Title = "Image Tap",
            IconImageSource = "csharp.png"
        });

        // demonstrates adding GestureRecognizer to a Border
        tabs.Children.Add(new TapInsideFrame
        {
            Title = "Border Tap",
            IconImageSource = "csharp.png"
        });

        // demonstrates using Xaml, Command and databinding
        tabs.Children.Add(new TapInsideFrameXaml
        {
            Title = "XAML Binding",
            IconImageSource = "xaml.png"
        });

        return new Window(tabs)
        {
            Title = "Tap Gesture Demo - .NET MAUI"
        };
    }
}
namespace TapGesture;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var tabs = new TabbedPage();

        // demonstrates an Image tap (and changing the image)
        tabs.Children.Add(new TapInsideImage { Title = "Image", IconImageSource = "csharp.png" });

        // demonstrates adding GestureRecognizer to a Frame
        tabs.Children.Add(new TapInsideFrame { Title = "Frame", IconImageSource = "csharp.png" });

        // demonstrates using Xaml, Command and databinding
        tabs.Children.Add(new TapInsideFrameXaml { Title = "In Xaml", IconImageSource = "xaml.png" });

        return new Window(tabs);
    }
}
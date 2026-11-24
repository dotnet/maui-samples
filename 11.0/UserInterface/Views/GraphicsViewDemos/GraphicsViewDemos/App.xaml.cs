namespace GraphicsViewDemos;

public partial class App : Application
{
    public static Color PrimaryColor { get; private set; }
    public static Color SecondaryColor { get; private set; }
    public static Color TertiaryColor { get; private set; }

    public App()
  	{
    		InitializeComponent();

    		PrimaryColor = GetColorFromResource("Primary");
    		SecondaryColor = GetColorFromResource("Secondary");
    		TertiaryColor = GetColorFromResource("Tertiary");
  	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }

  	Color GetColorFromResource(string resourceName)
  	{
    		object color;
    		App.Current.Resources.TryGetValue(resourceName, out color);
    		return (Color)color;
  	}
}

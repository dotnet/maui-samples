namespace GraphicsViewDemos;

public partial class App : Application
{
    public static Color PrimaryColor { get; private set; }
    public static Color SecondaryColor { get; private set; }
    public static Color TertiaryColor { get; private set; }

    public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

		PrimaryColor = GetColorFromResource("Primary");
		SecondaryColor = GetColorFromResource("Secondary");
		TertiaryColor = GetColorFromResource("Tertiary");
	}

	Color GetColorFromResource(string resourceName)
	{
		object color;
		App.Current.Resources.TryGetValue(resourceName, out color);
		return (Color)color;
	}
}

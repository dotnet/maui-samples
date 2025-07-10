namespace WorkingWithColors;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        // Register routes for navigation
        Routing.RegisterRoute(nameof(ColorDemo), typeof(ColorDemo));
        Routing.RegisterRoute(nameof(ColorsInXaml), typeof(ColorsInXaml));
	}
}

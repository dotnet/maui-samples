using LocalChatClientWithAgents.Pages;

namespace LocalChatClientWithAgents;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		// Only TripPlanningPage is navigable - LandmarkTripView is a child component
		Routing.RegisterRoute(nameof(TripPlanningPage), typeof(TripPlanningPage));
	}
}

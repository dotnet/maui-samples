using LocalChatClientWithAgents.Pages;

namespace LocalChatClientWithAgents;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(TripPlanningPage), typeof(TripPlanningPage));
		Routing.RegisterRoute(nameof(ItineraryPage), typeof(ItineraryPage));
	}
}

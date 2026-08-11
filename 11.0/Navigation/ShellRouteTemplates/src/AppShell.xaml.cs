namespace ShellRouteTemplates;

public partial class AppShell : Shell
{
	public AppShell(MainPage mainPage)
	{
		InitializeComponent();
		RouteMatrix.Content = mainPage;

		Routing.RegisterRoute("trip/{tripId}", typeof(RequiredResultPage));
		Routing.RegisterRoute("traveler/{name?}", typeof(TravelerResultPage));
		Routing.RegisterRoute("rating/{stars=5}", typeof(DefaultValueResultPage));
		Routing.RegisterRoute("reservation/{reservationId:int}", typeof(IntConstraintResultPage));
		Routing.RegisterRoute("loyalty/{points:long}", typeof(LongConstraintResultPage));
		Routing.RegisterRoute("budget/{amount:double}", typeof(DoubleConstraintResultPage));
		Routing.RegisterRoute("toggle/{enabled:bool}", typeof(BoolConstraintResultPage));
		Routing.RegisterRoute("booking/{reference:guid}", typeof(GuidConstraintResultPage));
		Routing.RegisterRoute("region/{name:alpha}", typeof(AlphaConstraintResultPage));
		Routing.RegisterRoute("files/{*path}", typeof(CatchAllResultPage));
		Routing.RegisterRoute("trip-{tripId}-summary", typeof(MixedResultPage));
	}
}

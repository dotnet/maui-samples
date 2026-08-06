namespace ShellRouteTemplates.Models;

public sealed record RouteExample(
	string Id,
	string Form,
	string Template,
	string NavigationUri,
	string Delivery,
	string ParameterName,
	string ExpectedValue,
	string Summary)
{
	public string AutomationId => $"Route-{Id}";
}

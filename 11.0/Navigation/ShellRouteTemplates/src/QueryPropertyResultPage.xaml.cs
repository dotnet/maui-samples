using ShellRouteTemplates.Models;

namespace ShellRouteTemplates;

[QueryProperty(nameof(TripId), "tripId")]
[QueryProperty(nameof(CaseId), "caseId")]
public partial class QueryPropertyResultPage : ContentPage
{
	string? caseId;
	string? tripId;

	protected QueryPropertyResultPage(string automationId)
	{
		InitializeComponent();
		ActualLabel.AutomationId = $"ActualValue-{automationId}";
		StatusLabel.AutomationId = $"ResultStatus-{automationId}";
	}

	public string? CaseId
	{
		get => caseId;
		set
		{
			caseId = value;
			Render();
		}
	}

	public string? TripId
	{
		get => tripId;
		set
		{
			tripId = value;
			Render();
		}
	}

	void Render()
	{
		if (caseId is null || tripId is null)
		{
			return;
		}

		RouteResultView.Render(
			RouteCatalog.Find(caseId),
			tripId,
			FormLabel,
			TemplateLabel,
			DeliveryLabel,
			ExpectedLabel,
			ActualLabel,
			StatusLabel);
	}

	async void OnBackToMatrix(object? sender, EventArgs e) =>
		await Shell.Current.GoToAsync("//routes");
}

public sealed class RequiredResultPage : QueryPropertyResultPage
{
	public RequiredResultPage() : base("required")
	{
	}
}

public sealed class MixedResultPage : QueryPropertyResultPage
{
	public MixedResultPage() : base("mixed")
	{
	}
}

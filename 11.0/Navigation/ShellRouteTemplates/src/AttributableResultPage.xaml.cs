using ShellRouteTemplates.Models;

namespace ShellRouteTemplates;

public partial class AttributableResultPage : ContentPage, IQueryAttributable
{
	protected AttributableResultPage()
	{
		InitializeComponent();
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		var caseId = query.TryGetValue("caseId", out var caseValue)
			? caseValue?.ToString()
			: null;
		var example = RouteCatalog.Find(caseId);
		var actualValue = example is not null
			&& query.TryGetValue(example.ParameterName, out var parameterValue)
				? parameterValue?.ToString()
				: null;
		var actualLabel = new Label();
		var statusLabel = new Label
		{
			FontAttributes = FontAttributes.Bold,
			FontSize = 24
		};

		if (example is not null)
		{
			actualLabel.AutomationId = $"ActualValue-{example.Id}";
			statusLabel.AutomationId = $"ResultStatus-{example.Id}";
		}

		ActualValueHost.Content = actualLabel;
		StatusHost.Content = statusLabel;

		RouteResultView.Render(
			example,
			actualValue,
			FormLabel,
			TemplateLabel,
			DeliveryLabel,
			ExpectedLabel,
			actualLabel,
			statusLabel);
	}

	async void OnBackToMatrix(object? sender, EventArgs e) =>
		await Shell.Current.GoToAsync("//routes");
}

public sealed class TravelerResultPage : AttributableResultPage
{
}

public sealed class DefaultValueResultPage : AttributableResultPage
{
}

public sealed class IntConstraintResultPage : AttributableResultPage
{
}

public sealed class LongConstraintResultPage : AttributableResultPage
{
}

public sealed class DoubleConstraintResultPage : AttributableResultPage
{
}

public sealed class BoolConstraintResultPage : AttributableResultPage
{
}

public sealed class GuidConstraintResultPage : AttributableResultPage
{
}

public sealed class AlphaConstraintResultPage : AttributableResultPage
{
}

public sealed class CatchAllResultPage : AttributableResultPage
{
}

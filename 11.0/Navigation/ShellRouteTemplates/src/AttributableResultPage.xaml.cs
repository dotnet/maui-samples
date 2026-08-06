using ShellRouteTemplates.Models;

namespace ShellRouteTemplates;

public partial class AttributableResultPage : ContentPage, IQueryAttributable
{
	protected AttributableResultPage(string automationId)
	{
		InitializeComponent();
		ActualLabel.AutomationId = $"ActualValue-{automationId}";
		StatusLabel.AutomationId = $"ResultStatus-{automationId}";
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

		RouteResultView.Render(
			example,
			actualValue,
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

public sealed class TravelerResultPage : AttributableResultPage
{
	public TravelerResultPage() : base("traveler")
	{
	}
}

public sealed class DefaultValueResultPage : AttributableResultPage
{
	public DefaultValueResultPage() : base("default")
	{
	}
}

public sealed class IntConstraintResultPage : AttributableResultPage
{
	public IntConstraintResultPage() : base("constraint-int")
	{
	}
}

public sealed class LongConstraintResultPage : AttributableResultPage
{
	public LongConstraintResultPage() : base("constraint-long")
	{
	}
}

public sealed class DoubleConstraintResultPage : AttributableResultPage
{
	public DoubleConstraintResultPage() : base("constraint-double")
	{
	}
}

public sealed class BoolConstraintResultPage : AttributableResultPage
{
	public BoolConstraintResultPage() : base("constraint-bool")
	{
	}
}

public sealed class GuidConstraintResultPage : AttributableResultPage
{
	public GuidConstraintResultPage() : base("constraint-guid")
	{
	}
}

public sealed class AlphaConstraintResultPage : AttributableResultPage
{
	public AlphaConstraintResultPage() : base("constraint-alpha")
	{
	}
}

public sealed class CatchAllResultPage : AttributableResultPage
{
	public CatchAllResultPage() : base("catch-all")
	{
	}
}

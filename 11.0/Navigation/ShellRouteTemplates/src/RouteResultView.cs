using ShellRouteTemplates.Models;

namespace ShellRouteTemplates;

internal static class RouteResultView
{
	public static void Render(
		RouteExample? example,
		string? actualValue,
		Label formLabel,
		Label templateLabel,
		Label deliveryLabel,
		Label expectedLabel,
		Label actualLabel,
		Label statusLabel)
	{
		if (example is null)
		{
			statusLabel.Text = "FAIL: route case metadata was not delivered";
			statusLabel.TextColor = Colors.Red;
			return;
		}

		var normalizedActual = string.IsNullOrEmpty(actualValue)
			? RouteCatalog.NotSupplied
			: actualValue;
		var passed = normalizedActual == example.ExpectedValue;

		formLabel.Text = example.Form;
		templateLabel.Text = example.Template;
		deliveryLabel.Text = $"{example.Delivery}: {example.ParameterName}";
		expectedLabel.Text = example.ExpectedValue;
		actualLabel.Text = normalizedActual;
		statusLabel.Text = passed ? "PASS" : "FAIL";
		statusLabel.TextColor = passed ? Colors.Green : Colors.Red;
	}
}

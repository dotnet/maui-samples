using System.Text;
using DeveloperBalance.Models;
using DeveloperBalance.Services;

namespace DeveloperBalance.Pages.Controls;

public partial class CategoryChart
{
	public CategoryChart()
    {
        InitializeComponent();
        SemanticUpdateService.ChartSemanticsNeedsUpdate += OnChartSemanticsNeedsUpdate;
    }

    void OnChartSemanticsNeedsUpdate(object? sender, EventArgs e)
    {
		var items = doughnutSeries.ItemsSource as List<CategoryChartData>;

		var sb = new StringBuilder();
		sb.Append($"This task category circular doughnut chart visually represents {items?.Count ?? '0'} categories");

		if (items is not null)
		{
			sb.Append(": ");
			for (int i = 0; i < items.Count; i++)
			{
				if (i < items.Count - 1)
					sb.Append($"{items[i].Title} (value {items[i].Count}), ");
				else
					sb.Append($"and {items[i].Title} (value {items[i].Count}).");
			}

			sb.Append(" Each category is color-coded in the chart.");
		}

        SemanticProperties.SetDescription(chartBorder, sb.ToString());
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            // Unsubscribe when the control is disposed
            SemanticUpdateService.ChartSemanticsNeedsUpdate -= OnChartSemanticsNeedsUpdate;
        }
    }
}
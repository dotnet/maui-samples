namespace DeveloperBalance.Services;

public static class SemanticUpdateService
{
    public static event EventHandler? ChartSemanticsNeedsUpdate;

    public static void UpdateChartSemantics()
    {
        ChartSemanticsNeedsUpdate?.Invoke(null, EventArgs.Empty);
    }
}

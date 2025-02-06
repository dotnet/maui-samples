namespace DeveloperBalance.Pages.Controls;

public partial class CategoryChart
{
	public CategoryChart()
	{
		InitializeComponent();
	}

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var chart = Chart.Handler?.PlatformView;

#if WINDOWS
        if (chart is Microsoft.Maui.Platform.ContentPanel contentPanel)
        {
            contentPanel.IsTabStop = true;
        }
#endif
    }
} 
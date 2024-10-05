namespace LocalizationDemo;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        var layoutDirection = AppInfo.Current.RequestedLayoutDirection;
	}

    void OnRTLToggled(object sender, ToggledEventArgs e)
    {
        if (FlowDirection != FlowDirection.RightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;
    }
}

namespace procrastinate.Services;

/// <summary>
/// Attached property to enable click tracking on buttons.
/// Usage in XAML: services:ClickTracking.IsEnabled="True"
/// </summary>
public static class ClickTracking
{
    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.CreateAttached(
            "IsEnabled",
            typeof(bool),
            typeof(ClickTracking),
            false,
            propertyChanged: OnIsEnabledChanged);

    public static bool GetIsEnabled(BindableObject view) => (bool)view.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(BindableObject view, bool value) => view.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Button button)
        {
            if ((bool)newValue)
            {
                button.Clicked += OnButtonClicked;
            }
            else
            {
                button.Clicked -= OnButtonClicked;
            }
        }
    }

    private static void OnButtonClicked(object? sender, EventArgs e)
    {
        StatsService.Instance?.IncrementClicks();
    }
}

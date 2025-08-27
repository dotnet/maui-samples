namespace FlyoutPageNavigation;

public class FlyoutPageItem
{
    public string Title { get; set; } = string.Empty;

    public string IconSource { get; set; } = string.Empty;

    public Type TargetType { get; set; } = typeof(Page);
}
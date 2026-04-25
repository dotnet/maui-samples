namespace procrastinate.Services;

public class ClickTrackingBehavior : Behavior<Button>
{
    protected override void OnAttachedTo(Button button)
    {
        base.OnAttachedTo(button);
        button.Clicked += OnButtonClicked;
    }

    protected override void OnDetachingFrom(Button button)
    {
        button.Clicked -= OnButtonClicked;
        base.OnDetachingFrom(button);
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        StatsService.Instance?.IncrementClicks();
    }
}

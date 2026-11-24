namespace WeatherTwentyOne.Views;

public partial class WidgetsPanel
{
    public WidgetsPanel()
    {
        InitializeComponent();
    }

    async void OnTapped(object sender, EventArgs eventArgs)
    {
        Grid g = (Grid)sender;

        await g.FadeToAsync(0, 200);
        await g.FadeToAsync(0.5, 100);
        await g.FadeToAsync(0, 100);
        await g.FadeToAsync(0.3, 100);
        await g.FadeToAsync(0, 100);

        await Task.Delay(1000);

        await g.FadeToAsync(1, 400);

    }
}

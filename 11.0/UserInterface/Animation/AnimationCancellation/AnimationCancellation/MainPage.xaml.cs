namespace AnimationCancellation;

public partial class MainPage : ContentPage
{
    CancellationTokenSource? _cts;

    public MainPage()
    {
        InitializeComponent();
    }

    // <docregion_fade_with_token>
    async void OnFadeClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        FadeButton.IsEnabled = false;
        CancelButton.IsEnabled = true;
        StatusLabel.Text = "Fading...";

        // FadeToAsync returns Task<bool>: true means the animation was
        // canceled, false means it ran to completion. The method does not
        // throw on cancellation.
        bool canceled = await LogoImage.FadeToAsync(0, 5000, Easing.SinIn, _cts.Token);

        StatusLabel.Text = canceled
            ? $"Canceled at opacity {LogoImage.Opacity:F2}."
            : "Faded out.";

        FadeButton.IsEnabled = true;
        CancelButton.IsEnabled = false;
    }

    void OnCancelClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
    }
    // </docregion_fade_with_token>

    void OnResetClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        LogoImage.Opacity = 1;
        StatusLabel.Text = "Ready.";
    }
}

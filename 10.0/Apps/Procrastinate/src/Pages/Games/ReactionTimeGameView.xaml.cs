using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class ReactionTimeGameView : ContentView
{
    private DateTime _startTime;
    private bool _waiting, _ready;
    private CancellationTokenSource? _cts;
    
    public Action? OnGamePlayed { get; set; }

    public ReactionTimeGameView()
    {
        InitializeComponent();
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        StartBtn.IsEnabled = false;
        ReactionBtn.IsEnabled = true;
        ReactionBtn.BackgroundColor = Colors.Red;
        ReactionBtn.Text = AppStrings.GetString("Wait");
        ResultLabel.Text = AppStrings.GetString("WaitForGreen");
        _waiting = true;
        _ready = false;

        var delay = Random.Shared.Next(2000, 5000);
        try
        {
            await Task.Delay(delay, _cts.Token);
            _waiting = false;
            _ready = true;
            _startTime = DateTime.Now;
            ReactionBtn.BackgroundColor = Colors.Green;
            ReactionBtn.Text = AppStrings.GetString("TapNow");
        }
        catch (TaskCanceledException) { }
    }

    private void OnReactionClicked(object? sender, EventArgs e)
    {
        if (_waiting)
        {
            _cts?.Cancel();
            ResultLabel.Text = AppStrings.GetString("TooEarly");
            ReactionBtn.BackgroundColor = Colors.Orange;
            ReactionBtn.Text = AppStrings.GetString("TooSoon");
            StartBtn.IsEnabled = true;
            ReactionBtn.IsEnabled = false;
        }
        else if (_ready)
        {
            var time = (DateTime.Now - _startTime).TotalMilliseconds;
            ResultLabel.Text = AppStrings.GetString("ReactionTime", (int)time);
            ReactionBtn.BackgroundColor = Colors.Gray;
            ReactionBtn.Text = AppStrings.GetString("Done");
            StartBtn.IsEnabled = true;
            ReactionBtn.IsEnabled = false;
            _ready = false;
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
        _waiting = false;
        _ready = false;
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class ClickSpeedGameView : ContentView, INotifyPropertyChanged
{
    private int _clickCount;
    private bool _active;
    private string _scoreText = "";
    
    public Action? OnGamePlayed { get; set; }
    public Action<int>? OnHighScore { get; set; }

    public string ScoreText
    {
        get => _scoreText;
        set { _scoreText = value; OnPropertyChanged(); }
    }

    public ClickSpeedGameView()
    {
        InitializeComponent();
        BindingContext = this;
        ScoreText = AppStrings.GetString("Clicks", 0);
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _clickCount = 0;
        ScoreText = AppStrings.GetString("Clicks", 0);
        StartBtn.IsEnabled = false;
        ClickBtn.IsEnabled = true;
        _active = true;

        await Task.Delay(5000);

        _active = false;
        ClickBtn.IsEnabled = false;
        StartBtn.IsEnabled = true;
        ScoreText = AppStrings.GetString("FinalClicks", _clickCount, _clickCount / 5.0);
        OnHighScore?.Invoke(_clickCount);
    }

    private void OnClickBtnClicked(object? sender, EventArgs e)
    {
        if (!_active) return;
        _clickCount++;
        ScoreText = AppStrings.GetString("Clicks", _clickCount);
    }

    public void Stop()
    {
        _active = false;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

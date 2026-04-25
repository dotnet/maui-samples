using Plugin.Maui.Audio;
using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class SimonSaysGameView : ContentView
{
    private readonly List<int> _sequence = [];
    private int _index;
    private bool _playerTurn;
    private int _score;
    private readonly Dictionary<int, IAudioPlayer?> _sounds = [];
    
    public Action? OnGamePlayed { get; set; }
    public Action<int>? OnHighScore { get; set; }

    public SimonSaysGameView()
    {
        InitializeComponent();
        UpdateScoreLabel();
        _ = LoadSoundsAsync();
    }

    private async Task LoadSoundsAsync()
    {
        var audioManager = AudioManager.Current;
        var soundFiles = new[] { "simon_red.wav", "simon_green.wav", "simon_blue.wav", "simon_yellow.wav" };
        
        for (int i = 0; i < soundFiles.Length; i++)
        {
            try
            {
                var stream = await FileSystem.OpenAppPackageFileAsync(soundFiles[i]);
                _sounds[i] = audioManager.CreatePlayer(stream);
            }
            catch
            {
                _sounds[i] = null;
            }
        }
    }

    private void PlaySound(int color)
    {
        if (_sounds.TryGetValue(color, out var player) && player != null)
        {
            player.Stop();
            player.Seek(0);
            player.Play();
        }
    }

    private void UpdateScoreLabel()
    {
        ScoreLabel.Text = AppStrings.GetString("Score", _score);
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _sequence.Clear();
        _score = 0;
        UpdateScoreLabel();
        StartBtn.IsEnabled = false;
        await AddToSequence();
    }

    private async Task AddToSequence()
    {
        _sequence.Add(Random.Shared.Next(4));
        await PlaySequence();
    }

    private async Task PlaySequence()
    {
        _playerTurn = false;
        SetButtonsEnabled(false);
        await Task.Delay(500);

        foreach (var color in _sequence)
        {
            var btn = GetButton(color);
            var original = btn.BackgroundColor;
            btn.BackgroundColor = GetBrightColor(color);
            PlaySound(color);
            await Task.Delay(400);
            btn.BackgroundColor = original;
            await Task.Delay(200);
        }

        _playerTurn = true;
        _index = 0;
        SetButtonsEnabled(true);
    }

    private void OnRedClicked(object? sender, EventArgs e) => OnColorClicked(0);
    private void OnGreenClicked(object? sender, EventArgs e) => OnColorClicked(1);
    private void OnBlueClicked(object? sender, EventArgs e) => OnColorClicked(2);
    private void OnYellowClicked(object? sender, EventArgs e) => OnColorClicked(3);

    private async void OnColorClicked(int color)
    {
        if (!_playerTurn) return;

        var btn = GetButton(color);
        var original = btn.BackgroundColor;
        btn.BackgroundColor = GetBrightColor(color);
        PlaySound(color);
        await Task.Delay(150);
        btn.BackgroundColor = original;

        if (color == _sequence[_index])
        {
            _index++;
            if (_index >= _sequence.Count)
            {
                _score++;
                UpdateScoreLabel();
                await Task.Delay(500);
                _ = AddToSequence();
            }
        }
        else
        {
            ScoreLabel.Text = AppStrings.GetString("GameOver", _score);
            OnHighScore?.Invoke(_score);
            StartBtn.IsEnabled = true;
            SetButtonsEnabled(false);
        }
    }

    private Button GetButton(int i) => i switch { 0 => RedBtn, 1 => GreenBtn, 2 => BlueBtn, _ => YellowBtn };
    private static Color GetBrightColor(int i) => i switch { 0 => Colors.Red, 1 => Colors.Lime, 2 => Colors.Blue, _ => Colors.Yellow };
    private void SetButtonsEnabled(bool e) { RedBtn.IsEnabled = e; GreenBtn.IsEnabled = e; BlueBtn.IsEnabled = e; YellowBtn.IsEnabled = e; }

    public void Stop() => _playerTurn = false;
}

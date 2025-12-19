using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class NumberGuessingGameView : ContentView
{
    private int _target;
    private int _attempts;
    private bool _gameOver;
    
    public Action? OnGamePlayed { get; set; }

    public NumberGuessingGameView()
    {
        InitializeComponent();
        ResetGame();
    }

    private void ResetGame()
    {
        OnGamePlayed?.Invoke();
        _target = Random.Shared.Next(1, 101);
        _attempts = 0;
        _gameOver = false;
        ResultLabel.Text = AppStrings.GetString("ThinkingOfNumber");
        ResultLabel.TextColor = Color.FromArgb("#D8DEE9");
        AttemptsLabel.Text = AppStrings.GetString("Attempts", 0);
        GuessEntry.Text = "";
        GuessEntry.IsEnabled = true;
        GuessBtn.IsEnabled = true;
    }

    private void OnGuessClicked(object? sender, EventArgs e)
    {
        if (_gameOver) return;
        if (!int.TryParse(GuessEntry.Text, out int guess) || guess < 1 || guess > 100)
        {
            ResultLabel.Text = AppStrings.GetString("EnterNumber1to100");
            return;
        }

        _attempts++;
        AttemptsLabel.Text = AppStrings.GetString("Attempts", _attempts);

        if (guess == _target)
        {
            ResultLabel.Text = AppStrings.GetString("Correct", _target);
            ResultLabel.TextColor = Color.FromArgb("#A3BE8C");
            _gameOver = true;
            GuessEntry.IsEnabled = false;
            GuessBtn.IsEnabled = false;
        }
        else if (guess < _target)
        {
            ResultLabel.Text = AppStrings.GetString("TooLow", guess);
            ResultLabel.TextColor = Color.FromArgb("#81A1C1");
        }
        else
        {
            ResultLabel.Text = AppStrings.GetString("TooHigh", guess);
            ResultLabel.TextColor = Color.FromArgb("#BF616A");
        }

        GuessEntry.Text = "";
    }

    private void OnNewGameClicked(object? sender, EventArgs e) => ResetGame();

    public void Stop() => _gameOver = true;
}

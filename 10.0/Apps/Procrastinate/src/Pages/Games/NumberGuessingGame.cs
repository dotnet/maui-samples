using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class NumberGuessingGame : MiniGame
{
    public override string Name => AppStrings.GetString("NumberGuess");
    public override string Icon => "\uf059";
    public override string IconColor => "#D08770";
    public override string Description => AppStrings.GetString("NumberGuessDesc");

    private NumberGuessingGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new NumberGuessingGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

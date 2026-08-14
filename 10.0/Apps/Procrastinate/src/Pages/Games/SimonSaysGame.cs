using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class SimonSaysGame : MiniGame
{
    public override string Name => AppStrings.GetString("SimonSays");
    public override string Icon => "\uf111";
    public override string IconColor => "#BF616A";
    public override string Description => AppStrings.GetString("SimonSaysDesc");

    private SimonSaysGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new SimonSaysGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke(),
            OnHighScore = score => OnHighScore?.Invoke(Name, score)
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

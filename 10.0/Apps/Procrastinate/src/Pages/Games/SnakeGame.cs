using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class SnakeGame : MiniGame
{
    public override string Name => AppStrings.GetString("TiltSnake");
    public override string Icon => "\uf7a0";
    public override string IconColor => "#88C0D0";
    public override string Description => AppStrings.GetString("TiltSnakeDesc");

    private SnakeGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new SnakeGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke(),
            OnHighScore = score => OnHighScore?.Invoke(Name, score)
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

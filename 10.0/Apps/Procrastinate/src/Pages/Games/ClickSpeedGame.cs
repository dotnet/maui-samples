using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class ClickSpeedGame : MiniGame
{
    public override string Name => AppStrings.GetString("ClickSpeed");
    public override string Icon => "\uf0e7";
    public override string IconColor => "#D08770";
    public override string Description => AppStrings.GetString("ClickSpeedDesc");

    private ClickSpeedGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new ClickSpeedGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke(),
            OnHighScore = score => OnHighScore?.Invoke(Name, score)
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

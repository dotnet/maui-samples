using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class WhackAMoleGame : MiniGame
{
    public override string Name => AppStrings.GetString("WhackAMole");
    public override string Icon => "\uf6d3";
    public override string IconColor => "#B48EAD";
    public override string Description => AppStrings.GetString("WhackAMoleDesc");

    private WhackAMoleGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new WhackAMoleGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke(),
            OnHighScore = score => OnHighScore?.Invoke(Name, score)
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

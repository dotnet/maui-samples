using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class ReactionTimeGame : MiniGame
{
    public override string Name => AppStrings.GetString("ReactionTimeGame");
    public override string Icon => "\uf192";
    public override string IconColor => "#BF616A";
    public override string Description => AppStrings.GetString("ReactionTimeDesc");

    private ReactionTimeGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new ReactionTimeGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class MemoryMatchGame : MiniGame
{
    public override string Name => AppStrings.GetString("MemoryMatch");
    public override string Icon => "\uf5fd";
    public override string IconColor => "#B48EAD";
    public override string Description => AppStrings.GetString("MemoryMatchDesc");

    private MemoryMatchGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new MemoryMatchGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

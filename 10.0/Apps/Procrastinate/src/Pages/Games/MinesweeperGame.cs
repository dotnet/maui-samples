using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class MinesweeperGame : MiniGame
{
    public override string Name => AppStrings.GetString("Minesweeper");
    public override string Icon => "\uf1e2";
    public override string IconColor => "#A3BE8C";
    public override string Description => AppStrings.GetString("MinesweeperDesc");

    private MinesweeperGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new MinesweeperGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

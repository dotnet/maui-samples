using procrastinate.Services;

namespace procrastinate.Pages.Games;

public class TicTacToeGame : MiniGame
{
    public override string Name => AppStrings.GetString("TicTacToe");
    public override string Icon => "\uf00a";
    public override string IconColor => "#B48EAD";
    public override string Description => AppStrings.GetString("TicTacToeDesc");

    private TicTacToeGameView? _gameView;

    public TicTacToeAI? AI { get; set; }

    public override View CreateGameView()
    {
        _gameView = new TicTacToeGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        if (AI is not null) _gameView.SetAI(AI);
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}

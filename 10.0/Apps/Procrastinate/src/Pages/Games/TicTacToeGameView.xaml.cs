using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class TicTacToeGameView : ContentView
{
    private readonly Button[] _cells = new Button[9];
    private readonly string[] _board = new string[9];
    private bool _playerTurn = true;
    private bool _gameOver;
    private int _gamesPlayed;
    
    public Action? OnGamePlayed { get; set; }

    public TicTacToeGameView()
    {
        InitializeComponent();
        _gamesPlayed = Preferences.Get("TicTacToeGamesPlayed", 0);
        CreateCells();
        ResetGame();
    }

    private void CreateCells()
    {
        for (int i = 0; i < 9; i++)
        {
            var idx = i;
            _cells[i] = new Button
            {
                BackgroundColor = Color.FromArgb("#434C5E"),
                TextColor = Colors.White,
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 8
            };
            _cells[i].Clicked += (s, e) => OnCellClicked(idx);
            GameGrid.Add(_cells[i], i % 3, i / 3);
        }
    }

    private void ResetGame()
    {
        OnGamePlayed?.Invoke();
        for (int i = 0; i < 9; i++)
        {
            _board[i] = "";
            _cells[i].Text = "";
            _cells[i].IsEnabled = true;
        }
        _playerTurn = true;
        _gameOver = false;
        StatusLabel.Text = AppStrings.GetString("YourTurn");
    }

    private void OnResetClicked(object? sender, EventArgs e) => ResetGame();

    private async void OnCellClicked(int idx)
    {
        if (_gameOver || !string.IsNullOrEmpty(_board[idx])) return;

        _board[idx] = "X";
        _cells[idx].Text = "X";
        _cells[idx].TextColor = Color.FromArgb("#88C0D0");

        if (CheckWin("X"))
        {
            StatusLabel.Text = AppStrings.GetString("YouWin");
            _gameOver = true;
            IncrementGamesPlayed();
            return;
        }

        if (IsBoardFull())
        {
            StatusLabel.Text = AppStrings.GetString("Draw");
            _gameOver = true;
            IncrementGamesPlayed();
            return;
        }

        _playerTurn = false;
        StatusLabel.Text = TicTacToeAI.IsAvailable 
            ? AppStrings.GetString("AIThinking") + " 🤖" 
            : AppStrings.GetString("AIThinking");
        AIDebugLabel.Text = "";
        await Task.Delay(300);

        var aiMove = await GetAIMoveAsync();
        
        // Show AI debug info
        AIDebugLabel.Text = TicTacToeAI.LastDebugInfo;
        
        if (aiMove >= 0)
        {
            _board[aiMove] = "O";
            _cells[aiMove].Text = "O";
            _cells[aiMove].TextColor = Color.FromArgb("#BF616A");

            if (CheckWin("O"))
            {
                StatusLabel.Text = AppStrings.GetString("AIWins");
                _gameOver = true;
                IncrementGamesPlayed();
                return;
            }

            if (IsBoardFull())
            {
                StatusLabel.Text = AppStrings.GetString("Draw");
                _gameOver = true;
                IncrementGamesPlayed();
                return;
            }
        }

        _playerTurn = true;
        StatusLabel.Text = AppStrings.GetString("YourTurn");
    }

    private int GetAIMove()
    {
        var empty = Enumerable.Range(0, 9).Where(i => string.IsNullOrEmpty(_board[i])).ToList();
        if (empty.Count == 0) return -1;

        // Play randomly for the first 2 games, then use strategy
        if (_gamesPlayed < 2)
        {
            return empty[Random.Shared.Next(empty.Count)];
        }

        // Strategy: Win if possible
        var winMove = FindWinningMove("O");
        if (winMove >= 0) return winMove;

        // Strategy: Block player's winning move
        var blockMove = FindWinningMove("X");
        if (blockMove >= 0) return blockMove;

        // Strategy: Take center if available
        if (string.IsNullOrEmpty(_board[4])) return 4;

        // Strategy: Take a corner
        int[] corners = { 0, 2, 6, 8 };
        var availableCorners = corners.Where(c => string.IsNullOrEmpty(_board[c])).ToList();
        if (availableCorners.Count > 0)
            return availableCorners[Random.Shared.Next(availableCorners.Count)];

        // Take any available edge
        int[] edges = { 1, 3, 5, 7 };
        var availableEdges = edges.Where(e => string.IsNullOrEmpty(_board[e])).ToList();
        if (availableEdges.Count > 0)
            return availableEdges[Random.Shared.Next(availableEdges.Count)];

        // Fallback: random
        return empty[Random.Shared.Next(empty.Count)];
    }

    private async Task<int> GetAIMoveAsync()
    {
        // Try real AI if available
        if (TicTacToeAI.IsAvailable)
        {
            var aiMove = await TicTacToeAI.GetMoveAsync(_board);
            if (aiMove >= 0) return aiMove;
        }
        
        // Fall back to built-in strategy
        return GetAIMove();
    }

    private int FindWinningMove(string player)
    {
        int[,] wins = { {0,1,2}, {3,4,5}, {6,7,8}, {0,3,6}, {1,4,7}, {2,5,8}, {0,4,8}, {2,4,6} };
        
        for (int i = 0; i < 8; i++)
        {
            int[] line = { wins[i,0], wins[i,1], wins[i,2] };
            int playerCount = line.Count(idx => _board[idx] == player);
            int emptyCount = line.Count(idx => string.IsNullOrEmpty(_board[idx]));
            
            // If player has 2 in a row and 1 empty, that empty cell is a winning/blocking move
            if (playerCount == 2 && emptyCount == 1)
            {
                return line.First(idx => string.IsNullOrEmpty(_board[idx]));
            }
        }
        return -1;
    }

    private void IncrementGamesPlayed()
    {
        _gamesPlayed++;
        Preferences.Set("TicTacToeGamesPlayed", _gamesPlayed);
    }

    private bool CheckWin(string player)
    {
        int[,] wins = { {0,1,2}, {3,4,5}, {6,7,8}, {0,3,6}, {1,4,7}, {2,5,8}, {0,4,8}, {2,4,6} };
        for (int i = 0; i < 8; i++)
            if (_board[wins[i,0]] == player && _board[wins[i,1]] == player && _board[wins[i,2]] == player)
                return true;
        return false;
    }

    private bool IsBoardFull() => _board.All(c => !string.IsNullOrEmpty(c));

    public void Stop() => _gameOver = true;
}

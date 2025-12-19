using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class MemoryMatchGameView : ContentView
{
    private readonly string[] _symbols = ["🐶", "🐱", "🦊", "🐸", "🍎", "🍊", "🍇", "🍓"];
    private readonly Button[] _cards = new Button[16];
    private readonly string[] _cardValues = new string[16];
    private readonly bool[] _matched = new bool[16];
    private int _firstCard = -1;
    private int _secondCard = -1;
    private bool _checking;
    private int _moves;
    private int _pairsFound;
    
    public Action? OnGamePlayed { get; set; }

    public MemoryMatchGameView()
    {
        InitializeComponent();
        CreateCards();
        ResetGame();
    }

    private void CreateCards()
    {
        for (int i = 0; i < 16; i++)
        {
            var idx = i;
            _cards[i] = new Button
            {
                BackgroundColor = Color.FromArgb("#434C5E"),
                TextColor = Colors.White,
                FontSize = 24,
                CornerRadius = 8,
                Text = "?"
            };
            _cards[i].Clicked += (s, e) => OnCardClicked(idx);
            GameGrid.Add(_cards[i], i % 4, i / 4);
        }
    }

    private void ResetGame()
    {
        OnGamePlayed?.Invoke();
        _moves = 0;
        _pairsFound = 0;
        _firstCard = -1;
        _secondCard = -1;
        _checking = false;

        var values = new List<string>();
        foreach (var s in _symbols) { values.Add(s); values.Add(s); }
        
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        for (int i = 0; i < 16; i++)
        {
            _cardValues[i] = values[i];
            _matched[i] = false;
            _cards[i].Text = "?";
            _cards[i].BackgroundColor = Color.FromArgb("#434C5E");
            _cards[i].IsEnabled = true;
        }

        UpdateStatus();
    }

    private void OnResetClicked(object? sender, EventArgs e) => ResetGame();

    private async void OnCardClicked(int idx)
    {
        if (_checking || _matched[idx] || idx == _firstCard) return;

        _cards[idx].Text = _cardValues[idx];
        _cards[idx].BackgroundColor = Color.FromArgb("#3B4252");

        if (_firstCard == -1)
        {
            _firstCard = idx;
        }
        else
        {
            _secondCard = idx;
            _moves++;
            _checking = true;
            UpdateStatus();

            await Task.Delay(600);

            if (_cardValues[_firstCard] == _cardValues[_secondCard])
            {
                _matched[_firstCard] = true;
                _matched[_secondCard] = true;
                _cards[_firstCard].BackgroundColor = Color.FromArgb("#88C0D0");
                _cards[_secondCard].BackgroundColor = Color.FromArgb("#88C0D0");
                _pairsFound++;
                UpdateStatus();

                if (_pairsFound == 8)
                    StatusLabel.Text = AppStrings.GetString("WinInMoves", _moves);
            }
            else
            {
                _cards[_firstCard].Text = "?";
                _cards[_secondCard].Text = "?";
                _cards[_firstCard].BackgroundColor = Color.FromArgb("#434C5E");
                _cards[_secondCard].BackgroundColor = Color.FromArgb("#434C5E");
            }

            _firstCard = -1;
            _secondCard = -1;
            _checking = false;
        }
    }

    private void UpdateStatus()
    {
        StatusLabel.Text = AppStrings.GetString("MovesAndPairs", _moves, _pairsFound, 8);
    }

    public void Stop() => _checking = false;
}

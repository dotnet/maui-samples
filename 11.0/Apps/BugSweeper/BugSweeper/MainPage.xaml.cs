namespace BugSweeper;

public partial class MainPage : ContentPage
{
    const string timeFormat = @"%m\:ss";

    bool isGameInProgress;
    DateTime gameStartTime;

    public MainPage()
    {
        InitializeComponent();

        board.GameStarted += (sender, args) =>
        {
            isGameInProgress = true;
            gameStartTime = DateTime.Now;

            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                timeLabel.Text = (DateTime.Now - gameStartTime).ToString(timeFormat);
                return isGameInProgress;
            });
        };

        board.GameEnded += (sender, hasWon) =>
        {
            isGameInProgress = false;

            if (hasWon)
                DisplayWonAnimation();
            else
                DisplayLostAnimation();
        };

        PrepareForNewGame();
    }

    void PrepareForNewGame()
    {
        board.NewGameInitialize();

        congratulationsText.IsVisible = false;
        consolationText.IsVisible = false;
        playAgainButton.IsVisible = false;
        playAgainButton.IsEnabled = false;

        timeLabel.Text = new TimeSpan().ToString(timeFormat);
        isGameInProgress = false;
    }

    // Maintains a square aspect ratio for the board.
    void OnBoardGridSizeChanged(object sender, EventArgs args)
    {
        Grid grid = (Grid)sender;
        double width = grid.Width;
        double height = grid.Height;
        double dimension = Math.Min(width, height);
        double horzPadding = (width - dimension) / 2;
        double vertPadding = (height - dimension) / 2;
        grid.Padding = new Thickness(horzPadding, vertPadding);
    }

    async void DisplayWonAnimation()
    {
        congratulationsText.Scale = 0;
        congratulationsText.IsVisible = true;

        // Because IsVisible has been false, the text might not have a size yet,
        // in which case Measure will return a size.
        double congratulationsTextWidth = congratulationsText.Measure(Double.PositiveInfinity, Double.PositiveInfinity).Width;

        congratulationsText.Rotation = 0;
        congratulationsText.RotateToAsync(3 * 360, 1000, Easing.CubicOut);

        double maxScale = 0.9 * board.Width / congratulationsTextWidth;
        await congratulationsText.ScaleToAsync(maxScale, 1000);

        foreach (View view in congratulationsText.Children)
        {
            view.Rotation = 0;
            view.RotateToAsync(180);
            await view.ScaleToAsync(3, 100);
            view.RotateToAsync(360);
            await view.ScaleToAsync(1, 100);
        }

        await DisplayPlayAgainButton();
    }

    async void DisplayLostAnimation()
    {
        consolationText.Scale = 0;
        consolationText.IsVisible = true;

        // Because IsVisible has been false, the text might not have a size yet,
        // in which case Measure will return a size.
        double consolationTextWidth = consolationText.Measure(Double.PositiveInfinity, Double.PositiveInfinity).Width;

        double maxScale = 0.9 * board.Width / consolationTextWidth;
        await consolationText.ScaleToAsync(maxScale, 5000);
        await DisplayPlayAgainButton();
    }

    async Task DisplayPlayAgainButton()
    {
        playAgainButton.Scale = 0;
        playAgainButton.IsVisible = true;
        playAgainButton.IsEnabled = true;

        // Because IsVisible has been false, the text might not have a size yet,
        // in which case Measure will return a size.
        double playAgainButtonWidth = playAgainButton.Measure(Double.PositiveInfinity, Double.PositiveInfinity).Width;

        double maxScale = board.Width / playAgainButtonWidth;
        await playAgainButton.ScaleToAsync(maxScale, 1000, Easing.SpringOut);
    }

    void OnplayAgainButtonClicked(object sender, object EventArgs)
    {
        PrepareForNewGame();
    }
}

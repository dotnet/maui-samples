namespace WordPuzzle;

public partial class MainPage : ContentPage
{
    // Number of squares horizontally and vertically, but some code will break if you change it.
    static readonly int NUM = 4;

    // Array of GameSquare views, and empty row & column.
    GameSquare[,] squares = new GameSquare[NUM, NUM];
    int emptyRow = NUM - 1;
    int emptyCol = NUM - 1;

    double squareSize;
    bool isBusy;
    bool isPlaying;

    public MainPage()
    {
        InitializeComponent();

        // Create GameSquare's for all the rows and columns.
        string text = ".NET MAUI ROCKS";
        string winText = "CONGRATULATIONS";
        int index = 0;

        for (int row = 0; row < NUM; row++)
        {
            for (int col = 0; col < NUM; col++)
            {
                // But skip the last one!
                if (row == NUM - 1 && col == NUM - 1)
                    break;

                // Instantiate GameSquare.
                GameSquare square = new GameSquare(text[index], winText[index], index)
                {
                    Row = row,
                    Col = col
                };

                // Add tap recognition
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command(OnSquareTapped),
                    CommandParameter = square
                };
                square.GestureRecognizers.Add(tapGestureRecognizer);

                // Add it to the array and the AbsoluteLayout.
                squares[row, col] = square;
                absoluteLayout.Add(square);
                index++;
            }
        }
    }

    void OnStackSizeChanged(object sender, EventArgs args)
    {
        double width = stackLayout.Width;
        double height = stackLayout.Height;

        if (width <= 0 || height <= 0)
            return;

        // Orient StackLayout based on portrait/landscape mode.
        stackLayout.Orientation = (width < height) ? StackOrientation.Vertical :
                                                     StackOrientation.Horizontal;

        // Calculate square size and position based on stack size.
        squareSize = Math.Min(width, height) / NUM;
        absoluteLayout.WidthRequest = NUM * squareSize;
        absoluteLayout.HeightRequest = NUM * squareSize;

#if ANDROID || IOS
        double multiplier = 0.4;
#elif WINDOWS
        double multiplier = 0.5;
#elif MACCATALYST
        double multiplier = 0.6;
#endif

        foreach (View view in absoluteLayout)
        {
            GameSquare square = (GameSquare)view;
            square.SetLabelFont(multiplier * squareSize, FontAttributes.Bold);

            AbsoluteLayout.SetLayoutBounds(square, new Rect(square.Col * squareSize,
                                                            square.Row * squareSize,
                                                            squareSize,
                                                            squareSize));
        }
    }

    async void OnSquareTapped(object parameter)
    {
        if (isBusy)
            return;

        isBusy = true;
        GameSquare tappedSquare = (GameSquare)parameter;
        await ShiftIntoEmpty(tappedSquare.Row, tappedSquare.Col);
        isBusy = false;

        // Check for a "win".
        if (isPlaying)
        {
            int index;
            for (index = 0; index < NUM * NUM - 1; index++)
            {
                int row = index / NUM;
                int col = index % NUM;
                GameSquare square = squares[row, col];
                if (square == null || square.Index != index)
                    break;
            }

            // We have a winner!
            if (index == NUM * NUM - 1)
            {
                isPlaying = false;
                await DoWinAnimation();
            }
        }
    }

    async Task ShiftIntoEmpty(int tappedRow, int tappedCol, uint length = 100)
    {
        // Shift columns
        if (tappedRow == emptyRow && tappedCol != emptyCol)
        {
            int inc = Math.Sign(tappedCol - emptyCol);
            int begCol = emptyCol + inc;
            int endCol = tappedCol + inc;

            for (int col = begCol; col != endCol; col += inc)
            {
                await AnimateSquare(emptyRow, col, emptyRow, emptyCol, length);
            }
        }
        // Shift rows
        else if (tappedCol == emptyCol && tappedRow != emptyRow)
        {
            int inc = Math.Sign(tappedRow - emptyRow);
            int begRow = emptyRow + inc;
            int endRow = tappedRow + inc;

            for (int row = begRow; row != endRow; row += inc)
            {
                await AnimateSquare(row, emptyCol, emptyRow, emptyCol, length);
            }
        }
    }

    async Task AnimateSquare(int row, int col, int newRow, int newCol, uint length)
    {
        // The Square to be animated.
        GameSquare animaSquare = squares[row, col];

        // The destination rectangle.
        Rect rect = new Rect(squareSize * emptyCol,
                                      squareSize * emptyRow,
                                      squareSize,
                                      squareSize);

        // This is the actual animation call.
        await animaSquare.LayoutTo(rect, length);

        // Set layout bounds to same Rectangle.
        AbsoluteLayout.SetLayoutBounds(animaSquare, rect);

        // Set several variables and properties for new layout.
        squares[newRow, newCol] = animaSquare;
        animaSquare.Row = newRow;
        animaSquare.Col = newCol;
        squares[row, col] = null;
        emptyRow = row;
        emptyCol = col;
    }

    async void OnRandomizeButtonClicked(object sender, EventArgs args)
    {
        Button button = (Button)sender;
        button.IsEnabled = false;
        Random rand = new Random();

        isBusy = true;

        // Simulate some fast crazy taps.
        for (int i = 0; i < 100; i++)
        {
            await ShiftIntoEmpty(rand.Next(NUM), emptyCol, 25);
            await ShiftIntoEmpty(emptyRow, rand.Next(NUM), 25);
        }
        button.IsEnabled = true;

        isBusy = false;

        // Prepare for playing.
        DateTime startTime = DateTime.Now;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            // Round duration and get rid of milliseconds.
            TimeSpan timeSpan = (DateTime.Now - startTime) + TimeSpan.FromSeconds(0.5);
            timeSpan = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            // Display the duration.
            if (isPlaying)
                timeLabel.Text = timeSpan.ToString("t");

            return isPlaying;
        });
        this.isPlaying = true;
    }

    async Task DoWinAnimation()
    {
        // Inhibit all input.
        randomizeButton.IsEnabled = false;
        isBusy = true;

        for (int cycle = 0; cycle < 2; cycle++)
        {
            foreach (GameSquare square in squares)
            {
                if (square != null)
                    await square.AnimateWinAsync(cycle == 1);
            }

            if (cycle == 0)
                await Task.Delay(1500);
        }

        // All input.
        randomizeButton.IsEnabled = true;
        isBusy = false;
    }
}

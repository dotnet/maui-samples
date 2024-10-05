namespace GameOfLife;

public partial class MainPage : ContentPage
{
    const int MaxCellSize = 30;     // includes cell spacing
    const int CellSpacing = 2;

    // Generating too many BoxView elements can impact performance, 
    //      particularly on iOS devices.
    const int MaxCellCount = 400;

    // Calculated during SizeChanged event 
    int cols;
    int rows;
    int cellSize;
    int xMargin;
    int yMargin;

    GameGrid gameGrid = new GameGrid();
    bool isRunning;

    public MainPage()
    {
        InitializeComponent();
    }

    void OnLayoutSizeChanged(object sender, EventArgs args)
    {
        Layout layout = sender as Layout;

        cols = (int)Math.Round(layout.Width / MaxCellSize);
        rows = (int)Math.Round(layout.Height / MaxCellSize);

        if (cols * rows > MaxCellCount)
        {
            cellSize = (int)Math.Sqrt((layout.Width * layout.Height) / MaxCellCount);
            cols = (int)(layout.Width / cellSize);
            rows = (int)(layout.Height / cellSize);
        }
        else
            cellSize = (int)Math.Min(layout.Width / cols, layout.Height / rows);

        xMargin = (int)((layout.Width - cols * cellSize) / 2);
        yMargin = (int)((layout.Height - rows * cellSize) / 2);

        if (cols > 0 && rows > 0)
        {
            gameGrid.SetSize(cols, rows);
            UpdateLayout();
            UpdateLives();
        }
    }

    void UpdateLayout()
    {
        int count = rows * cols;
        System.Diagnostics.Debug.WriteLine("Count = " + count);

        // Remove unneeded LifeCell children
        while (absoluteLayout.Children.Count > count)
        {
            absoluteLayout.Children.RemoveAt(0);
        }

        // Possibly add more LifeCell children
        while (absoluteLayout.Children.Count < count)
        {
            GameCell gameCell = new GameCell();
            gameCell.Tapped += OnTapGestureTapped;
            absoluteLayout.Children.Add(gameCell);
        }

        int index = 0;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameCell gameCell = gameCell = (GameCell)absoluteLayout.Children[index];
                gameCell.Col = x;
                gameCell.Row = y;
                gameCell.IsAlive = gameGrid.IsAlive(x, y);

                Rect rect = new Rect(x * cellSize + xMargin + CellSpacing / 2,
                    y * cellSize + yMargin + CellSpacing / 2,
                    cellSize - CellSpacing,
                    cellSize - CellSpacing);

                AbsoluteLayout.SetLayoutBounds(gameCell, rect);
                index++;
            }
        }
    }

    void UpdateLives()
    {
        foreach (View view in absoluteLayout.Children)
        {
            GameCell gameCell = view as GameCell;
            gameCell.IsAlive = gameGrid.IsAlive(gameCell.Col, gameCell.Row);
        }
    }

    void OnTapGestureTapped(object sender, EventArgs args)
    {
        GameCell gameCell = (GameCell)sender;
        gameCell.IsAlive ^= true;
        gameGrid.SetStatus(gameCell.Col, gameCell.Row, gameCell.IsAlive);
    }

    void OnRunButtonClicked(object sender, EventArgs args)
    {
        if (!isRunning)
        {
            runButton.Text = "Pause";
            isRunning = true;
            clearButton.IsEnabled = false;
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(250), OnTimerTick);
        }
        else
            StopRunning();
    }

    void StopRunning()
    {
        isRunning = false;
        runButton.Text = "Run!";
        clearButton.IsEnabled = true;
    }

    bool OnTimerTick()
    {
        if (isRunning)
        {
            bool isLifeLeft = gameGrid.Tick();
            UpdateLives();

            if (!isLifeLeft)
                StopRunning();
        }
        return isRunning;
    }

    void OnClearButtonClicked(object sender, EventArgs args)
    {
        gameGrid.Clear();
        UpdateLives();
    }

    async void OnAboutButtonClicked(object sender, EventArgs args)
    {
        await Navigation.PushModalAsync(new AboutPage());
    }
}
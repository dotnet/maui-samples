using Microsoft.Maui.Controls.Shapes;

namespace BugSweeper
{
    enum TileStatus
    {
        Hidden,
        Flagged,
        Exposed
    }

    class Tile : Border
    {
        TileStatus tileStatus = TileStatus.Hidden;
        Label label;
        Image flagImage, bugImage;
        bool doNotFireEvent;

#if WINDOWS
        bool lastTapSingle;
        DateTime lastTapTime;
#endif

        public event EventHandler<TileStatus> TileStatusChanged;

        public int Row { get; private set; }
        public int Col { get; private set; }
        public bool IsBug { get; set; }
        public int SurroundingBugCount { get; set; }

        public TileStatus Status
        {
            get => tileStatus;
            set
            {
                if (tileStatus != value)
                {
                    tileStatus = value;

                    switch (tileStatus)
                    {
                        case TileStatus.Hidden:
                            this.Content = null;
                            break;

                        case TileStatus.Flagged:
                            this.Content = flagImage;
                            break;

                        case TileStatus.Exposed:
                            if (this.IsBug)
                                this.Content = bugImage;
                            else
                            {
                                this.Content = label;
                                label.Text = (this.SurroundingBugCount > 0) ? this.SurroundingBugCount.ToString() : " ";
                            }
                            break;
                    }

                    if (!doNotFireEvent && TileStatusChanged != null)
                        TileStatusChanged(this, tileStatus);
                }
            }
        }

        public Tile(int row, int col)
        {
            this.Row = row;
            this.Col = col;

            this.BackgroundColor = Color.FromArgb("#512BD4");
            this.Stroke = Colors.Black;
            this.StrokeThickness = 2;
            this.StrokeShape = new RoundRectangle
            {
                CornerRadius = 4
            };
            this.Padding = 2;

            label = new Label
            {
                Text = " ",
                TextColor = Colors.Black,
                BackgroundColor = Colors.Gold,
                FontSize = 24,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            flagImage = new Image { Source = ImageSource.FromFile("dotnet_logo.png") };
            bugImage = new Image { Source = ImageSource.FromFile("redbug.png") };

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            this.GestureRecognizers.Add(singleTap);

#if ANDROID || IOS || MACCATALYST
            TapGestureRecognizer doubleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2
            };
            doubleTap.Tapped += OnDoubleTap;
            this.GestureRecognizers.Add(doubleTap);
#endif
        }

        // Does not fire TileStatusChanged events.
        public void Initialize()
        {
            doNotFireEvent = true;
            this.Status = TileStatus.Hidden;
            this.IsBug = false;
            this.SurroundingBugCount = 0;
            doNotFireEvent = false;
        }

        void OnSingleTap(object sender, object args)
        {
#if WINDOWS
            // Simulate double tap on Windows
            if (lastTapSingle && DateTime.Now - lastTapTime < TimeSpan.FromMilliseconds(500))
            {
                OnDoubleTap(sender, args);
                lastTapSingle = false;
            }
            else
            {
                lastTapTime = DateTime.Now;
                lastTapSingle = true;
            }
#endif
            switch (this.Status)
            {
                case TileStatus.Hidden:
                    this.Status = TileStatus.Flagged;
                    break;

                case TileStatus.Flagged:
                    this.Status = TileStatus.Hidden;
                    break;

                case TileStatus.Exposed:
                    // Do nothing
                    break;
            }
        }

        void OnDoubleTap(object sender, object args)
        {
            this.Status = TileStatus.Exposed;
        }
    }
}

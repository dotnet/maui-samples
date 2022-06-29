using GradientStop = Microsoft.Maui.Controls.GradientStop;

namespace BrushesDemos.Views
{
    public partial class UpdateBrushDemoPage : ContentPage
    {
        readonly Random random;
        Timer timer;

        public UpdateBrushDemoPage()
        {
            InitializeComponent();
            random = new Random();

            UpdateBrushes();

            timer = new Timer(new TimerCallback((s) =>
                MainThread.BeginInvokeOnMainThread(() => UpdateBrushes())),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));
        }

        ~UpdateBrushDemoPage() => timer.Dispose();

        void UpdateBrushes()
        {
            Color randomColor1 = GetRandomColor();
            Color randomColor2 = GetRandomColor();

            SolidColorBrush solidColorBrush = new SolidColorBrush
            {
                Color = randomColor1
            };

            LinearGradientBrush linearGradientBrush = new LinearGradientBrush
            {
                // StartPoint defaults to (0,0). EndPoint defaults to (1,1).
                GradientStops =
                {
                    new GradientStop { Color = randomColor1, Offset = 0.1f },
                    new GradientStop { Color = randomColor2, Offset = 1.0f }
                }
            };

            RadialGradientBrush radialGradientBrush = new RadialGradientBrush
            {
                // Center defaults to (0.5,0,5).
                Radius = 0.75,
                GradientStops =
                {
                    new GradientStop { Color = randomColor1, Offset = 0.1f },
                    new GradientStop { Color = randomColor2, Offset = 1.0f }
                }
            };

            solidColorFrame.Background = solidColorBrush;
            linearGradientFrame.Background = linearGradientBrush;
            radialGradientFrame.Background = radialGradientBrush;
        }

        Color GetRandomColor()
        {
            return Color.FromRgb((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
        }
    }
}


 
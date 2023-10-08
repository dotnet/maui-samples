using Microsoft.Maui.Layouts;

namespace AbsoluteLayoutDemos.Views.Code
{
    public class ChessboardDemoPage : ContentPage
    {
        AbsoluteLayout absoluteLayout;

        public ChessboardDemoPage()
        {
            absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Colors.WhiteSmoke,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            for (int row=0; row < 8; row++)
            {
                for (int col=0; col < 8; col++)
                {
                    // Skip every other square
                    if (((row ^ col) & 1) == 0)
                    {
                        continue;
                    }

                    BoxView boxView = new BoxView
                    {
                        Color = Colors.Black
                    };

                    // x, y, width, height
                    Rect rect = new Rect(col / 7.0, row / 7.0, 1 / 8.0, 1 / 8.0);

                    absoluteLayout.Add(boxView, rect, AbsoluteLayoutFlags.All);
                }
            }

            ContentView contentView = new ContentView
            {
                Margin = new Thickness(20),
                Content = absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;

            Title = "Chessboard demo";
            Content = contentView;
        }

        void OnContentViewSizeChanged(object sender, EventArgs e)
        {
            ContentView contentView = sender as ContentView;
            double boardSize = Math.Min(contentView.Width, contentView.Height);
            absoluteLayout.WidthRequest = boardSize;
            absoluteLayout.HeightRequest = boardSize;
        }
    }
}


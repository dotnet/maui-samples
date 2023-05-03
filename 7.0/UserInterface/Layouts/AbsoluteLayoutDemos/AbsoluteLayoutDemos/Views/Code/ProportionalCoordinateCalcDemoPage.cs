using Microsoft.Maui.Layouts;

namespace AbsoluteLayoutDemos.Views.Code
{
    public class ProportionalCoordinateCalcDemoPage : ContentPage
    {
        AbsoluteLayout absoluteLayout;

        public ProportionalCoordinateCalcDemoPage()
        {
            absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Colors.LightCoral,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            ContentView contentView = new ContentView
            {
                Margin = new Thickness(20),
                Content = absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;

            Rect[] fractionalRects =
            {
                new Rect(0.05, 0.1, 0.90, 0.1),    // outer top
                new Rect(0.05, 0.8, 0.90, 0.1),    // outer bottom
                new Rect(0.05, 0.1, 0.05, 0.8),    // outer left
                new Rect(0.90, 0.1, 0.05, 0.8),    // outer right

                new Rect(0.15, 0.3, 0.70, 0.1),    // inner top
                new Rect(0.15, 0.6, 0.70, 0.1),    // inner bottom
                new Rect(0.15, 0.3, 0.05, 0.4),    // inner left
                new Rect(0.80, 0.3, 0.05, 0.4),    // inner right
            };

            foreach (Rect fractionalRect in fractionalRects)
            {
                Rect layoutBounds = new Rect
                {
                    // Proportional coordinate calculations
                    X = fractionalRect.X / (1 - fractionalRect.Width),
                    Y = fractionalRect.Y / (1 - fractionalRect.Height),
                    Width = fractionalRect.Width,
                    Height = fractionalRect.Height
                };

                absoluteLayout.Add(new BoxView
                {
                    Color = Colors.DarkBlue
                }, layoutBounds, AbsoluteLayoutFlags.All);
            }

            Title = "Proportional coordinate calculations demo";
            Content = contentView;
        }

        void OnContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;

            // Figure has an aspect ratio of 2:1
            double height = Math.Min(contentView.Width / 2, contentView.Height);
            absoluteLayout.WidthRequest = 2 * height;
            absoluteLayout.HeightRequest = height;
        }
    }
}


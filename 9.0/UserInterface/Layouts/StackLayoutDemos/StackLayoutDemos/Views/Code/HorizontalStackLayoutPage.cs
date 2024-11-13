namespace StackLayoutDemos.Views.Code
{
    public class HorizontalStackLayoutPage : ContentPage
    {
        public HorizontalStackLayoutPage()
        {
            Title = "Horizontal StackLayout demo";

            StackLayout stackLayout = new StackLayout
            {
                Margin = new Thickness(20),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center
            };

            stackLayout.Add(new BoxView { Color = Colors.Red, WidthRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Yellow, WidthRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Blue, WidthRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Green, WidthRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Orange, WidthRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Purple, WidthRequest = 40 });

            Content = stackLayout;
        }
    }
}

namespace StackLayoutDemos.Views.Code
{
    public class StackLayoutSpacingPage : ContentPage
    {
        public StackLayoutSpacingPage()
        {
            Title = "StackLayout Spacing demo";

            StackLayout stackLayout = new StackLayout
            {
                Margin = new Thickness(20),
                Spacing = 6
            };

            stackLayout.Add(new Label { Text = "Primary colors" });
            stackLayout.Add(new BoxView { Color = Colors.Red, HeightRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Yellow, HeightRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Blue, HeightRequest = 40 });
            stackLayout.Add(new Label { Text = "Secondary colors" });
            stackLayout.Add(new BoxView { Color = Colors.Green, HeightRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Orange, HeightRequest = 40 });
            stackLayout.Add(new BoxView { Color = Colors.Purple, HeightRequest = 40 });

            Content = stackLayout;
        }
    }
}

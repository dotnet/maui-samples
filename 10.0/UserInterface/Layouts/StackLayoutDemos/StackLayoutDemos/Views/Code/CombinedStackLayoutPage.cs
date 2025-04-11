namespace StackLayoutDemos.Views.Code
{
    public class CombinedStackLayoutPage : ContentPage
    {
        public CombinedStackLayoutPage()
        {
            Title = "Combined StackLayouts demo";

            Border border1 = new Border
            {
                Stroke = Colors.Black,
                Padding = new Thickness(5)
            };

            StackLayout border1StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            border1StackLayout.Add(new BoxView { Color = Colors.Red, WidthRequest = 40 });
            border1StackLayout.Add(new Label { Text = "Red", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            border1.Content = border1StackLayout;

            Border border2 = new Border
            {
                Stroke = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout border2StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            border2StackLayout.Add(new BoxView { Color = Colors.Yellow, WidthRequest = 40 });
            border2StackLayout.Add(new Label { Text = "Yellow", FontSize =  20, VerticalOptions = LayoutOptions.Center });
            border2.Content = border2StackLayout;

            Border border3 = new Border
            {
                Stroke = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout border3StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            border3StackLayout.Add(new BoxView { Color = Colors.Blue, WidthRequest = 40 });
            border3StackLayout.Add(new Label { Text = "Blue", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            border3.Content = border3StackLayout;

            Border border4 = new Border
            {
                Stroke = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout border4StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            border4StackLayout.Add(new BoxView { Color = Colors.Green, WidthRequest = 40 });
            border4StackLayout.Add(new Label { Text = "Green", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            border4.Content = border4StackLayout;

            Border border5 = new Border
            {
                Stroke = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout border5StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            border5StackLayout.Add(new BoxView { Color = Colors.Orange, WidthRequest = 40 });
            border5StackLayout.Add(new Label { Text = "Orange", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            border5.Content = border5StackLayout;

            Border border6 = new Border
            {
                Stroke = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout border6StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            border6StackLayout.Add(new BoxView { Color = Colors.Purple, WidthRequest = 40 });
            border6StackLayout.Add(new Label { Text = "Purple", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            border6.Content = border6StackLayout;

            StackLayout stackLayout = new StackLayout { Margin = new Thickness(20) };
            stackLayout.Add(new Label { Text = "Primary colors" });
            stackLayout.Add(border1);
            stackLayout.Add(border2);
            stackLayout.Add(border3);
            stackLayout.Add(new Label { Text = "Secondary colors" });
            stackLayout.Add(border4);
            stackLayout.Add(border5);
            stackLayout.Add(border6);

            Content = stackLayout;
        }
    }
}

namespace StackLayoutDemos.Views.Code
{
    public class CombinedStackLayoutPage : ContentPage
    {
        public CombinedStackLayoutPage()
        {
            Title = "Combined StackLayouts demo";

            Frame frame1 = new Frame
            {
                BorderColor = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout frame1StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            frame1StackLayout.Add(new BoxView { Color = Colors.Red, WidthRequest = 40 });
            frame1StackLayout.Add(new Label { Text = "Red", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            frame1.Content = frame1StackLayout;

            Frame frame2 = new Frame
            {
                BorderColor = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout frame2StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            frame2StackLayout.Add(new BoxView { Color = Colors.Yellow, WidthRequest = 40 });
            frame2StackLayout.Add(new Label { Text = "Yellow", FontSize =  20, VerticalOptions = LayoutOptions.Center });
            frame2.Content = frame2StackLayout;

            Frame frame3 = new Frame
            {
                BorderColor = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout frame3StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            frame3StackLayout.Add(new BoxView { Color = Colors.Blue, WidthRequest = 40 });
            frame3StackLayout.Add(new Label { Text = "Blue", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            frame3.Content = frame3StackLayout;

            Frame frame4 = new Frame
            {
                BorderColor = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout frame4StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            frame4StackLayout.Add(new BoxView { Color = Colors.Green, WidthRequest = 40 });
            frame4StackLayout.Add(new Label { Text = "Green", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            frame4.Content = frame4StackLayout;

            Frame frame5 = new Frame
            {
                BorderColor = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout frame5StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            frame5StackLayout.Add(new BoxView { Color = Colors.Orange, WidthRequest = 40 });
            frame5StackLayout.Add(new Label { Text = "Orange", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            frame5.Content = frame5StackLayout;

            Frame frame6 = new Frame
            {
                BorderColor = Colors.Black,
                Padding = new Thickness(5)
            };
            StackLayout frame6StackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 15
            };
            frame6StackLayout.Add(new BoxView { Color = Colors.Purple, WidthRequest = 40 });
            frame6StackLayout.Add(new Label { Text = "Purple", FontSize = 20, VerticalOptions = LayoutOptions.Center });
            frame6.Content = frame6StackLayout;

            StackLayout stackLayout = new StackLayout { Margin = new Thickness(20) };
            stackLayout.Add(new Label { Text = "Primary colors" });
            stackLayout.Add(frame1);
            stackLayout.Add(frame2);
            stackLayout.Add(frame3);
            stackLayout.Add(new Label { Text = "Secondary colors" });
            stackLayout.Add(frame4);
            stackLayout.Add(frame5);
            stackLayout.Add(frame6);

            Content = stackLayout;
        }
    }
}

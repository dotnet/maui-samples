namespace StackLayoutDemos.Views.Code
{
    public class AlignmentPage : ContentPage
    {
        public AlignmentPage()
        {
            Title = "Alignment demo";

            StackLayout stackLayout = new StackLayout
            {
                Margin = new Thickness(20),
                Spacing = 6
            };

            stackLayout.Add(new Label { Text = "Start", BackgroundColor = Colors.Gray, HorizontalOptions = LayoutOptions.Start });
            stackLayout.Add(new Label { Text = "Center", BackgroundColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center });
            stackLayout.Add(new Label { Text = "End", BackgroundColor = Colors.Gray, HorizontalOptions = LayoutOptions.End });
            stackLayout.Add(new Label { Text = "Fill", BackgroundColor = Colors.Gray, HorizontalOptions = LayoutOptions.Fill });

            Content = stackLayout;
        }
    }
}

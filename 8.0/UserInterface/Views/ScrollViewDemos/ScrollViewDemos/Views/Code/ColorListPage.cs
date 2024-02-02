namespace ScrollViewDemos.Views.Code
{
    public class ColorListPage : ContentPage
    {
        public ColorListPage()
        {
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                BoxView boxView = new BoxView
                {
                    HeightRequest = 32,
                    WidthRequest = 32,
                    VerticalOptions = LayoutOptions.Center
                };
                boxView.SetBinding(BoxView.ColorProperty, "Color");

                Label label = new Label
                {
                    FontSize = 24,
                    VerticalOptions = LayoutOptions.Center
                };
                label.SetBinding(Label.TextProperty, "FriendlyName");

                StackLayout horizontalStackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                horizontalStackLayout.Add(boxView);
                horizontalStackLayout.Add(label);

                return horizontalStackLayout;
            });

            StackLayout stackLayout = new StackLayout();
            BindableLayout.SetItemsSource(stackLayout, NamedColor.All);
            BindableLayout.SetItemTemplate(stackLayout, dataTemplate);

            ScrollView scrollView = new ScrollView
            {
                Margin = new Thickness(20),
                Content = stackLayout
            };

            Title = "ScrollView demo";
            Content = scrollView;
        }
    }
}

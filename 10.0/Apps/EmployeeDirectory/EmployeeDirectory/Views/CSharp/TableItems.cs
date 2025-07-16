namespace EmployeeDirectory.Views.CSharp
{
    public class GroupHeaderTemplate : ViewCell
    {
        public GroupHeaderTemplate()
        {
            var label = new Label { VerticalTextAlignment = TextAlignment.Center };
            label.SetBinding(Label.TextProperty, "Title");
            View = new StackLayout
            {
                Padding = new Thickness(5, 0, 0, 0),
                Children = { label }
            };
        }
    }

    public class ListItemTemplate : ViewCell
    {
        public ListItemTemplate()
        {
            var photo = new Image { HeightRequest = 44, WidthRequest = 44 };
            photo.SetBinding(Image.SourceProperty, "Photo");

            var nameLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.None,
                FontSize = 16,
            };

            nameLabel.SetBinding(Label.TextProperty, "Name");

            var titleLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.None,
                FontSize = 12,
            };

            titleLabel.SetBinding(Label.TextProperty, "Title");

            var information = new StackLayout
            {
                Padding = new Thickness(5, 0, 0, 0),
                VerticalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Vertical,
                Children = { nameLabel, titleLabel }
            };

            View = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { photo, information }
            };
        }
    }

    public class DetailsItemTemplate : ViewCell
    {
        public DetailsItemTemplate()
        {
            var propertyNameLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 16,
                FontAttributes = FontAttributes.None,
            };

            propertyNameLabel.SetBinding(Label.TextProperty, "Name");

            var propertyValueLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
            propertyValueLabel.SetBinding(Label.TextProperty, "Value");

            View = new StackLayout
            {
                Padding = new Thickness(20, 0, 0, 0),
                VerticalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Horizontal,
                Children = { propertyNameLabel, propertyValueLabel }
            };
        }
    }
}


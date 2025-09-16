namespace EmployeeDirectory.Views.CSharp
{
    // Replaced obsolete ViewCell-based templates with plain layout factory helpers.
    public static class LegacyTemplates
    {
        public static View CreateGroupHeader()
        {
            var label = new Label { VerticalTextAlignment = TextAlignment.Center };
            label.SetBinding(Label.TextProperty, "Title");
            return new StackLayout
            {
                Padding = new Thickness(5, 0, 0, 0),
                Children = { label }
            };
        }

        public static View CreateListItem()
        {
            var photo = new Image { HeightRequest = 44, WidthRequest = 44 };
            photo.SetBinding(Image.SourceProperty, "Photo");

            var nameLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 16,
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");

            var titleLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
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

            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { photo, information }
            };
        }

        public static View CreateDetailsItem()
        {
            var propertyNameLabel = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 16,
            };
            propertyNameLabel.SetBinding(Label.TextProperty, "Name");

            var propertyValueLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
            propertyValueLabel.SetBinding(Label.TextProperty, "Value");

            return new StackLayout
            {
                Padding = new Thickness(20, 0, 0, 0),
                VerticalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Horizontal,
                Children = { propertyNameLabel, propertyValueLabel }
            };
        }
    }
}


namespace ModalNavigation;

public class DetailPageCS : ContentPage
{
    // Helper method to get a reasonable default font size for labels
    private static double GetDefaultLabelFontSize()
    {
        // You can adjust this value as needed for your app's design
        return 18;
    }

    public DetailPageCS()
    {
        var nameLabel = new Label
        {
            FontSize = GetDefaultLabelFontSize(),
            FontAttributes = FontAttributes.Bold
        };
        nameLabel.SetBinding(Label.TextProperty, "Name");

        var ageLabel = new Label
        {
            FontSize = GetDefaultLabelFontSize(),
            FontAttributes = FontAttributes.Bold
        };
        ageLabel.SetBinding(Label.TextProperty, "Age");

        var occupationLabel = new Label
        {
            FontSize = GetDefaultLabelFontSize(),
            FontAttributes = FontAttributes.Bold
        };
        occupationLabel.SetBinding(Label.TextProperty, "Occupation");

        var countryLabel = new Label
        {
            FontSize = GetDefaultLabelFontSize(),
            FontAttributes = FontAttributes.Bold
        };
        countryLabel.SetBinding(Label.TextProperty, "Country");

        var dismissButton = new Button { Text = "Dismiss" };
        dismissButton.Clicked += OnDismissButtonClicked;

        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        // Row 0: Name
        var nameKeyLabel = new Label
        {
            Text = "Name:",
            FontSize = GetDefaultLabelFontSize(),
            Margin = new Thickness(0, 0, 10, 0)
        };
        grid.Add(nameKeyLabel, 0, 0);
        grid.Add(nameLabel, 1, 0);

        // Row 1: Age
        var ageKeyLabel = new Label
        {
            Text = "Age:",
            FontSize = GetDefaultLabelFontSize(),
            Margin = new Thickness(0, 0, 10, 0)
        };
        grid.Add(ageKeyLabel, 0, 1);
        grid.Add(ageLabel, 1, 1);

        // Row 2: Occupation
        var occupationKeyLabel = new Label
        {
            Text = "Occupation:",
            FontSize = GetDefaultLabelFontSize(),
            Margin = new Thickness(0, 0, 10, 0)
        };
        grid.Add(occupationKeyLabel, 0, 2);
        grid.Add(occupationLabel, 1, 2);

        // Row 3: Country
        var countryKeyLabel = new Label
        {
            Text = "Country:",
            FontSize = GetDefaultLabelFontSize(),
            Margin = new Thickness(0, 0, 10, 0)
        };
        grid.Add(countryKeyLabel, 0, 3);
        grid.Add(countryLabel, 1, 3);

        // Row 4: Dismiss Button (spans both columns)
        grid.Add(dismissButton, 0, 4);
        Grid.SetColumnSpan(dismissButton, 2);

        Content = grid;
    }

    async void OnDismissButtonClicked(object? sender, EventArgs args)
    {
        await Navigation.PopModalAsync();
    }
}


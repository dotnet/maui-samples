namespace PassingData;

public class SecondPageCS : ContentPage
{
    public SecondPageCS()
    {
        var nameLabel = new Label
        {
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        };
        nameLabel.SetBinding(Label.TextProperty, "Name");

        var ageLabel = new Label
        {
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        };
        ageLabel.SetBinding(Label.TextProperty, "Age");

        var occupationLabel = new Label
        {
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        };
        occupationLabel.SetBinding(Label.TextProperty, "Occupation");

        var countryLabel = new Label
        {
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        };
        countryLabel.SetBinding(Label.TextProperty, "Country");

        var navigateButton = new Button { Text = "Previous Page" };
        navigateButton.Clicked += OnNavigateButtonClicked;

        var grid = new Grid
        {
            RowDefinitions =
            {
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

        grid.Add(new Label { Text = "Name:", FontSize = 18 }, 0, 0);
        grid.Add(nameLabel, 1, 0);
        
        grid.Add(new Label { Text = "Age:", FontSize = 18 }, 0, 1);
        grid.Add(ageLabel, 1, 1);
        
        grid.Add(new Label { Text = "Occupation:", FontSize = 18 }, 0, 2);
        grid.Add(occupationLabel, 1, 2);
        
        grid.Add(new Label { Text = "Country:", FontSize = 18 }, 0, 3);
        grid.Add(countryLabel, 1, 3);

        Title = "Second Page";
        Content = new StackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { grid, navigateButton }
        };
    }

    async void OnNavigateButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
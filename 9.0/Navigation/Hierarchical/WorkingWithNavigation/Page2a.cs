namespace WorkingWithNavigation;

public class Page2a : ContentPage
{
    public Page2a()
    {
        var nextPageButton = new Button { Text = "Next Page", VerticalOptions = LayoutOptions.Center };
        nextPageButton.Clicked += OnNextPageButtonClicked;

        var previousPageButton = new Button { Text = "Previous Page", VerticalOptions = LayoutOptions.Center };
        previousPageButton.Clicked += OnPreviousPageButtonClicked;

        Title = "Page 2a";
        Content = new StackLayout 
        { 
            Spacing = 20,
            Padding = new Thickness(20), // Add padding around the buttons
            Children =
            {
                nextPageButton,
                previousPageButton
            }
        };
    }

    async void OnNextPageButtonClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new Page3());
    }

    async void OnPreviousPageButtonClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
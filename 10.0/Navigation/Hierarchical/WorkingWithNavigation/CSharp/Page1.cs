namespace WorkingWithNavigation;

public class Page1 : ContentPage
{
    public Page1()
    {
        var nextPageButton = new Button { Text = "Next Page", VerticalOptions = LayoutOptions.Center };
        nextPageButton.Clicked += OnNextPageButtonClicked;

        Title = "Page 1";
        Content = new StackLayout 
        { 
            Spacing = 20,
            Padding = new Thickness(20), // Add padding around the button
            Children =
            {
                nextPageButton
            }
        };
    }

    async void OnNextPageButtonClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new Page2());
    }
}
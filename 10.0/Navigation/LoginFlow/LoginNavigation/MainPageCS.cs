namespace LoginNavigation;

public class MainPageCS : ContentPage
{
    public MainPageCS()
    {
        var toolbarItem = new ToolbarItem 
        {
            Text = "Logout"
        };
        toolbarItem.Clicked += OnLogoutButtonClicked;
        ToolbarItems.Add(toolbarItem);

        Title = "Main Page";
        Content = new StackLayout 
        { 
            Padding = new Thickness(20),
            Children =
            {
                new Label 
                {
                    Text = "Main app content goes here",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };
    }

    async void OnLogoutButtonClicked(object? sender, EventArgs e)
    {
        App.IsUserLoggedIn = false;
        Navigation.InsertPageBefore(new LoginPageCS(), this);
        await Navigation.PopAsync();
    }
}
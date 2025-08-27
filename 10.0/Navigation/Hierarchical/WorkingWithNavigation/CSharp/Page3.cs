namespace WorkingWithNavigation;

public class Page3 : ContentPage
{
    public Page3()
    {
        var previousPageButton = new Button { Text = "Previous Page", VerticalOptions = LayoutOptions.Center };
        previousPageButton.Clicked += OnPreviousPageButtonClicked;

        var rootPageButton = new Button { Text = "Return to Root Page", VerticalOptions = LayoutOptions.Center };
        rootPageButton.Clicked += OnRootPageButtonClicked;

        var insertPageButton = new Button 
        {
            Text = "Insert Page 2a Before Page 3",
            VerticalOptions = LayoutOptions.Center
        };
        insertPageButton.Clicked += OnInsertPageButtonClicked;

        var removePageButton = new Button { Text = "Remove Page 2", VerticalOptions = LayoutOptions.Center };
        removePageButton.Clicked += OnRemovePageButtonClicked;

        Title = "Page 3";
        Content = new StackLayout 
        { 
            Spacing = 20, // Add spacing between buttons
            Padding = new Thickness(20), // Add padding around the buttons
            Children =
            {
                previousPageButton,
                rootPageButton,
                insertPageButton,
                removePageButton
            }
        };
    }

    async void OnPreviousPageButtonClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void OnRootPageButtonClicked(object? sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    void OnInsertPageButtonClicked(object? sender, EventArgs e)
    {
        var page2a = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "Page 2a");
        if (page2a == null) 
        {
            Navigation.InsertPageBefore(new Page2a(), this);
        }
    }

    void OnRemovePageButtonClicked(object? sender, EventArgs e)
    {
        var page2 = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "Page 2");
        if (page2 != null) 
        {
            Navigation.RemovePage(page2);
        }
    }
}
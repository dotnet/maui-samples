using Microsoft.Maui.Controls.Shapes;
using EmployeeDirectory.Core.ViewModels;

namespace EmployeeDirectory.Views.CSharp;

public class LoginView : ContentPage
{
    private LoginViewModel? loginViewModel;

    private LoginViewModel Model
    {
        get
        {
            if (loginViewModel == null)
                loginViewModel = new LoginViewModel(App.Service!);

            return loginViewModel;
        }
    }

    public LoginView()
    {
        BindingContext = Model;

        var appName = new Label
        {
            Text = "Employee Directory",
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            TextColor = Application.Current?.Resources["Primary"] as Color ?? Color.FromArgb("#0066CC")
        };

        var logo = new Image { Source = FileImageSource.FromFile("logo.png"), HeightRequest = 100, WidthRequest = 100, HorizontalOptions = LayoutOptions.Center };

        var usernameEntry = new Entry { Placeholder = "Username", StyleId = "UserId", Margin = new Thickness(0, 10, 0, 0) };
        usernameEntry.SetBinding(Entry.TextProperty, "Username");

        var passwordEntry = new Entry { IsPassword = true, Placeholder = "Password", StyleId = "PassId", Margin = new Thickness(0, 10, 0, 0) };
        passwordEntry.SetBinding(Entry.TextProperty, "Password");

        var loginButton = new Button
        {
            Text = "Login",
            BackgroundColor = Application.Current?.Resources["Primary"] as Color ?? Color.FromArgb("#0066CC"),
            TextColor = Application.Current?.Resources["White"] as Color ?? Colors.White,
            CornerRadius = 24,
            HeightRequest = 48,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 20, 0, 0)
        };
        loginButton.Clicked += OnLoginClicked;

        var helpButton = new Button
        {
            Text = "Help",
            BackgroundColor = Colors.Transparent,
            TextColor = Application.Current?.Resources["Primary"] as Color ?? Color.FromArgb("#0066CC"),
            BorderColor = Application.Current?.Resources["Primary"] as Color ?? Color.FromArgb("#0066CC"),
            BorderWidth = 1,
            CornerRadius = 24,
            HeightRequest = 48,
            Margin = new Thickness(0, 10, 0, 0)
        };
        helpButton.Clicked += OnHelpClicked;

        var dotnetBot = new Image { Source = "dotnet_bot.png", HeightRequest = 80, WidthRequest = 80, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 0, 10) };

        var card = new Border
        {
            Padding = 24,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Application.Current?.Resources["White"] as Color ?? Colors.White,
            Stroke = Application.Current?.Resources["Gray300"] as Color ?? Color.FromArgb("#D1D1D6"),
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    appName,
                    dotnetBot,
                    logo,
                    usernameEntry,
                    passwordEntry,
                    loginButton,
                    helpButton
                }
            },
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        Content = new Grid
        {
            BackgroundColor = Application.Current?.Resources["Gray100"] as Color ?? Color.FromArgb("#F2F2F7"),
            Children = { card }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private void OnLoginClicked(object? sender, EventArgs e)
    {
        if (loginViewModel?.CanLogin == true)
        {
            loginViewModel
            .LoginAsync(System.Threading.CancellationToken.None)
            .ContinueWith(_ =>
            {
                App.LastUseTime = System.DateTime.UtcNow;
                Navigation.PopAsync();
            });

            Navigation.PopModalAsync();
        }
        else
        {
            DisplayAlert("Error", loginViewModel?.ValidationErrors ?? "Login failed", "OK");
        }
    }

    private void OnHelpClicked(object? sender, EventArgs e)
    {
        DisplayAlert("Help", "Enter any username and password", "OK");
    }
}

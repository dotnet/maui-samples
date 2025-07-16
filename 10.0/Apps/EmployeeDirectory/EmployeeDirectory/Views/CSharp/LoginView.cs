using System;
using EmployeeDirectory.ViewModels;
using DeviceInfo = Microsoft.Maui.Devices.DeviceInfo;
using DevicePlatform = Microsoft.Maui.Devices.DevicePlatform;

namespace EmployeeDirectory.Views.CSharp
{
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

            var logo = new Image { Source = FileImageSource.FromFile("logo.png") };

            var usernameEntry = new Entry { Placeholder = "Username", StyleId = "UserId" };
            usernameEntry.SetBinding(Entry.TextProperty, "Username");

            var passwordEntry = new Entry { IsPassword = true, Placeholder = "Password", StyleId = "PassId" };
            passwordEntry.SetBinding(Entry.TextProperty, "Password");

            var loginButton = new Button { Text = "Login" };
            loginButton.Clicked += OnLoginClicked;

            var helpButton = new Button { Text = "Help" };
            helpButton.Clicked += OnHelpClicked;

            var grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.Center
            };
            
            // Add column definitions
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                Grid.SetColumn(loginButton, 0);
                Grid.SetRow(loginButton, 0);
                grid.Children.Add(loginButton);
                
                Grid.SetColumn(helpButton, 1);
                Grid.SetRow(helpButton, 0);
                grid.Children.Add(helpButton);

                Content = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.Start,
                    Padding = new Thickness(30),
                    Children = { logo, usernameEntry, passwordEntry, grid }
                };

                BackgroundImageSource = "login_box";
            }
            else
            {
                Grid.SetColumn(logo, 0);
                Grid.SetRow(logo, 0);
                grid.Children.Add(logo);
                
                Grid.SetColumn(helpButton, 1);
                Grid.SetRow(helpButton, 0);
                grid.Children.Add(helpButton);

                Content = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(30),
                    Children = { grid, usernameEntry, passwordEntry, loginButton },
                };
            }
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
}

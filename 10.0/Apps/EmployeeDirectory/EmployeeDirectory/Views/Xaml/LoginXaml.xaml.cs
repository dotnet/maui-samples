namespace EmployeeDirectory.Views.Xaml;

public partial class LoginXaml : ContentPage
{
    private LoginViewModel viewModel;

    public LoginXaml()
    {
        InitializeComponent();
        viewModel = new LoginViewModel(App.Service);
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        string platformName = DeviceInfo.Platform.ToString();
        Content.FindByName<Button>("loginButton").Clicked += OnLoginClicked;
        Content.FindByName<Button>("helpButton").Clicked += OnHelpClicked;
    }

    private void OnLoginClicked(object? sender, EventArgs e)
    {
        if (viewModel.CanLogin)
        {
            viewModel
            .LoginAsync(CancellationToken.None)
            .ContinueWith(_ =>
            {
                App.LastUseTime = DateTime.UtcNow;
                Navigation.PopModalAsync();
            });

            Navigation.PopModalAsync();
        }
        else
        {
            DisplayAlert("Error", viewModel.ValidationErrors, "OK");
        }
    }

    private void OnHelpClicked(object? sender, EventArgs e)
    {
        DisplayAlert("Help", "Enter any username and password", "OK", null);
    }
}

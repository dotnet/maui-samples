using EmployeeDirectory.Core.ViewModels;

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

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        if (viewModel.CanLogin)
        {
            try
            {
                await viewModel.LoginAsync(CancellationToken.None);
                App.LastUseTime = DateTime.UtcNow;
                if (Navigation.ModalStack?.Any() == true)
                    await Navigation.PopModalAsync();
                else
                    await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        else
        {
            await DisplayAlertAsync("Error", viewModel.ValidationErrors, "OK");
        }
    }

    private async void OnHelpClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("Help", "Enter any username and password", "OK");
    }
}

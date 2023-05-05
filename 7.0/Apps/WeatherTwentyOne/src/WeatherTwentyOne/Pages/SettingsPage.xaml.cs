using WeatherTwentyOne.ViewModels;

namespace WeatherTwentyOne.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        BindingContext = new SettingsViewModel();
    }

    async void OnSignOut(object sender, EventArgs eventArgs)
    {
        await DisplayAlert("Sign Out", "Are you sure?", "Yes", "No");
    }

    async void OnSupportTapped(object sender, EventArgs eventArgs)
    {
        string action = await DisplayActionSheet("Get Help", "Cancel", null, "Email", "Chat", "Phone");
        await DisplayAlert("You Chose", action, "Okay");
    }

    void RadioButton_CheckedChanged(System.Object sender, CheckedChangedEventArgs e)
    {
        AppTheme val = (AppTheme)((RadioButton)sender).Value;
        if (App.Current.UserAppTheme == val)
            return;

        App.Current.UserAppTheme = val;
    }
}

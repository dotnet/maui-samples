using System.Diagnostics;
using WeatherTwentyOne.Pages;

namespace WeatherTwentyOne;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        //App.Current.UserAppTheme = AppTheme.Dark;

        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            Shell.Current.CurrentItem = PhoneTabs;

        //Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }

    async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        try { 
            await Shell.Current.GoToAsync($"///settings");
        }catch (Exception ex) {
            Debug.WriteLine($"err: {ex.Message}");
        }
    }
}

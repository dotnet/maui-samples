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

    void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        Shell.Current.GoToAsync($"///settings");
    }
}

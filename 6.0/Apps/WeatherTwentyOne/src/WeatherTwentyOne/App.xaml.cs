using WeatherTwentyOne.Pages;

namespace WeatherTwentyOne;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        //App.Current.UserAppTheme = OSAppTheme.Light;

        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            Shell.Current.CurrentItem = PhoneTabs;

        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }

    void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        Shell.Current.GoToAsync("///settings");
    }
}

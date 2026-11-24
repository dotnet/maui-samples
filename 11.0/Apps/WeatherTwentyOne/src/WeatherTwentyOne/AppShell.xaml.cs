using System.Diagnostics;

namespace WeatherTwentyOne;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            CurrentItem = PhoneTabs;
    }

    async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        try
        {
            await GoToAsync("///settings");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"err: {ex.Message}");
        }
    }
}

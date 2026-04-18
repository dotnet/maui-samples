using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DeveloperBalance;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override void OnBackPressed()
    {
        // Workaround for a .NET MAUI 9 bug where pressing Back from a root Shell page
        // destroys the Activity. On relaunch, CollectionView adapters still reference
        // the destroyed Activity context and attempt to load FontImageSource icons via
        // Glide, causing:
        //   java.lang.IllegalArgumentException: You cannot start a load for a destroyed activity
        //
        // Fix: when at the root of the navigation stack, move the app to the background
        // instead of finishing the Activity, preventing Activity destruction.
        // This is fixed in .NET 10: https://github.com/dotnet/maui/issues/29699
        var navStack = Shell.Current?.Navigation?.NavigationStack;
        if (navStack is null || navStack.Count <= 1)
        {
            MoveTaskToBack(true);
            return;
        }

        base.OnBackPressed();
    }
}

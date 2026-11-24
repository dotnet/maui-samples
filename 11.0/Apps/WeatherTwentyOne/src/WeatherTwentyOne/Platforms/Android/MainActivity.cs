using Android.App;
using Android.Content.PM;
using Android.OS;

namespace WeatherTwentyOne;

[IntentFilter(
    new[] { Platform.Intent.ActionAppAction },
	Categories = new[] { Android.Content.Intent.CategoryDefault })]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
    }

    protected override void OnResume()
    {
        base.OnResume();

        Platform.OnResume(this);
    }

    protected override void OnNewIntent(Android.Content.Intent intent)
	{
		base.OnNewIntent(intent);

        Platform.OnNewIntent(intent);
	}

	public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
	{
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

		base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	}
}
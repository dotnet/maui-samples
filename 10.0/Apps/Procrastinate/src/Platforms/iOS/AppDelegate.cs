using Foundation;
using UIKit;

namespace procrastinate;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
	{
		// Forward deep link to MAUI
		if (url.Scheme == "procrastinate" && url.Host is string route)
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await Task.Delay(500); // Wait for app to be ready
				await Shell.Current.GoToAsync($"//{route}");
			});
			return true;
		}
		return base.OpenUrl(application, url, options);
	}
}

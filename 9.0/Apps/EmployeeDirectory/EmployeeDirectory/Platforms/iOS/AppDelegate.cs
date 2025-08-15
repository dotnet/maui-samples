using Foundation;
using UIKit;

namespace EmployeeDirectory.Platforms.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var result = base.FinishedLaunching(application, launchOptions);
        
        // Set the phone feature service
        var window = UIApplication.SharedApplication.KeyWindow;
        var rootViewController = window?.RootViewController;
        if (rootViewController != null)
        {
            App.PhoneFeatureService = new iOSPhoneFeatureService(rootViewController);
        }
        
        return result;
    }
}
using Foundation;
using Microsoft.Maui;
using UIKit;

namespace EmployeeDirectory.Platforms.iOS;

[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        base.WillConnect(scene, session, connectionOptions);

        var rootViewController = Window?.RootViewController
            ?? throw new InvalidOperationException("The employee directory scene has no root view controller.");
        App.PhoneFeatureService = new iOSPhoneFeatureService(rootViewController);
    }
}

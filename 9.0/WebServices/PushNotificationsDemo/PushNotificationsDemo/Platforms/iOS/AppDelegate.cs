using System.Diagnostics;
using Foundation;
using PushNotificationsDemo.Platforms.iOS;
using PushNotificationsDemo.Services;
using UIKit;
using UserNotifications;

namespace PushNotificationsDemo;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    IPushDemoNotificationActionService _notificationActionService;
    INotificationRegistrationService _notificationRegistrationService;
    IDeviceInstallationService _deviceInstallationService;

    IPushDemoNotificationActionService NotificationActionService =>
        _notificationActionService ?? (_notificationActionService = IPlatformApplication.Current.Services.GetService<IPushDemoNotificationActionService>());

    INotificationRegistrationService NotificationRegistrationService =>
        _notificationRegistrationService ?? (_notificationRegistrationService = IPlatformApplication.Current.Services.GetService<INotificationRegistrationService>());

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ?? (_deviceInstallationService = IPlatformApplication.Current.Services.GetService<IDeviceInstallationService>());

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    [Export("application:didFinishLaunchingWithOptions:")]
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        if (DeviceInstallationService.NotificationsSupported)
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert |
                UNAuthorizationOptions.Badge |
                UNAuthorizationOptions.Sound,
                (approvalGranted, error) =>
                {
                    if (approvalGranted && error == null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            UIApplication.SharedApplication.RegisterForRemoteNotifications();
                        });
                    }
                });
        }

        using (var userInfo = launchOptions?.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary)
        {
            ProcessNotificationActions(userInfo);
        }

        return base.FinishedLaunching(application, launchOptions);
    }

    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        CompleteRegistrationAsync(deviceToken)
            .ContinueWith((task) =>
            {
                if (task.IsFaulted)
                    throw task.Exception;
            });
    }

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        Debug.WriteLine(error.Description);
    }

    [Export("application:didReceiveRemoteNotification:")]
    public void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
    {
        ProcessNotificationActions(userInfo);
    }

    Task CompleteRegistrationAsync(NSData deviceToken)
    {
        DeviceInstallationService.Token = deviceToken.ToHexString();
        return NotificationRegistrationService.RefreshRegistrationAsync();
    }

    void ProcessNotificationActions(NSDictionary userInfo)
    {
        if (userInfo == null)
            return;

        try
        {
            // If your app isn't in the foreground, the notification goes to Notification Center.
            // If your app is in the foreground, the notification goes directly to your app and you
            // need to process the notification payload yourself.

            var actionValue = userInfo.ObjectForKey(new NSString("action")) as NSString;

            if (!string.IsNullOrWhiteSpace(actionValue?.Description))
                NotificationActionService.TriggerAction(actionValue.Description);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}

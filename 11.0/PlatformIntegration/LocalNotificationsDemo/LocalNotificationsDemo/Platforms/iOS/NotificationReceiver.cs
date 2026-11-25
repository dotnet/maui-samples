using UserNotifications;

namespace LocalNotificationsDemo.Platforms.iOS;

public class NotificationReceiver : UNUserNotificationCenterDelegate
{
    // Called if app is in the foreground.
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        ProcessNotification(notification);

        var presentationOptions = (OperatingSystem.IsIOSVersionAtLeast(14))
            ? UNNotificationPresentationOptions.Banner
            : UNNotificationPresentationOptions.Alert;

        completionHandler(presentationOptions);
    }

    // Called if app is in the background, or killed state.
    public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
    {
        if (response.IsDefaultAction)
            ProcessNotification(response.Notification);

        completionHandler();
    }

    void ProcessNotification(UNNotification notification)
    {
        string title = notification.Request.Content.Title;
        string message = notification.Request.Content.Body;

        var service = IPlatformApplication.Current?.Services.GetService<INotificationManagerService>();
        service?.ReceiveNotification(title, message);
    }
}

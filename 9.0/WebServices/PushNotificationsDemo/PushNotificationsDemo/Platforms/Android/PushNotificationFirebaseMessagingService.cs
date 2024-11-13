using Android.App;
using Firebase.Messaging;
using PushNotificationsDemo.Services;

namespace PushNotificationsDemo.Platforms.Android;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
    IPushDemoNotificationActionService _notificationActionService;
    INotificationRegistrationService _notificationRegistrationService;
    IDeviceInstallationService _deviceInstallationService;
    int _messageId;

    IPushDemoNotificationActionService NotificationActionService =>
        _notificationActionService ?? (_notificationActionService = IPlatformApplication.Current.Services.GetService<IPushDemoNotificationActionService>());

    INotificationRegistrationService NotificationRegistrationService =>
        _notificationRegistrationService ?? (_notificationRegistrationService = IPlatformApplication.Current.Services.GetService<INotificationRegistrationService>());

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ?? (_deviceInstallationService = IPlatformApplication.Current.Services.GetService<IDeviceInstallationService>());

    public override void OnNewToken(string token)
    {
        DeviceInstallationService.Token = token;

        NotificationRegistrationService.RefreshRegistrationAsync()
            .ContinueWith((task) =>
            {
                if (task.IsFaulted)
                    throw task.Exception;
            });
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);

        if (message.Data.TryGetValue("action", out var messageAction))
            NotificationActionService.TriggerAction(messageAction);
    }
}

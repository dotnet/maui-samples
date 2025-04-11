namespace PushNotificationsDemo.Services;

public interface INotificationRegistrationService
{
    Task DeregisterDeviceAsync();
    Task RegisterDeviceAsync(params string[] tags);
    Task RefreshRegistrationAsync();
}


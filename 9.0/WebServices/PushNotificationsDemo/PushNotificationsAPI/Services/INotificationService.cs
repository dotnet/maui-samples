using PushNotificationsAPI.Models;

namespace PushNotificationsAPI.Services;

public interface INotificationService
{
    Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token);
    Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken token);
    Task<bool> RequestNotificationAsync(NotificationRequest notificationRequest, CancellationToken token);
}


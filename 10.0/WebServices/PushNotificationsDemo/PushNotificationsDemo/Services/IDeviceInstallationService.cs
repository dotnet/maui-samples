using PushNotificationsDemo.Models;

namespace PushNotificationsDemo.Services;

public interface IDeviceInstallationService
{
    string Token { get; set; }
    bool NotificationsSupported { get; }
    string GetDeviceId();
    DeviceInstallation GetDeviceInstallation(params string[] tags);
}


using PushNotificationsDemo.Models;

namespace PushNotificationsDemo.Services;

public interface IPushDemoNotificationActionService : INotificationActionService
{
    event EventHandler<PushDemoAction> ActionTriggered;
}


using PushNotificationsDemo.Models;
using PushNotificationsDemo.Services;

namespace PushNotificationsDemo;

public partial class App : Application
{
    readonly IPushDemoNotificationActionService _actionService;

    public App(IPushDemoNotificationActionService service)
    {
        InitializeComponent();

        _actionService = service;
        _actionService.ActionTriggered += NotificationActionTriggered;
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }

    void NotificationActionTriggered(object sender, PushDemoAction e)
    {
        ShowActionAlert(e);
    }

    void ShowActionAlert(PushDemoAction action)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Windows[0].Page?.DisplayAlert("Push notifications demo", $"{action} action received.", "OK")
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted)
                        throw task.Exception;
                });
        });
    }
}

using PushNotificationsDemo.Services;

namespace PushNotificationsDemo
{
    public partial class MainPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;

        public MainPage(INotificationRegistrationService service)
        {
            InitializeComponent();

            _notificationRegistrationService = service;
        }

#if ANDROID
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            PermissionStatus status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }
#endif

        void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            _notificationRegistrationService.RegisterDeviceAsync()
                .ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted ? task.Exception.Message : $"Device registered");
                });
        }

        void OnDeregisterButtonClicked(object sender, EventArgs e)
        {
            _notificationRegistrationService.DeregisterDeviceAsync()
                .ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted ? task.Exception.Message : $"Device deregistered");
                });
        }

        void ShowAlert(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("Push notifications demo", message, "OK")
                    .ContinueWith((task) =>
                    {
                        if (task.IsFaulted)
                            throw task.Exception;
                    });
            });
        }
    }
}

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace LocalNotificationsDemo
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            CreateNotificationFromIntent(intent);
        }

        static void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(LocalNotificationsDemo.Platforms.Android.NotificationManagerService.TitleKey);
                string message = intent.GetStringExtra(LocalNotificationsDemo.Platforms.Android.NotificationManagerService.MessageKey);

                var service = IPlatformApplication.Current?.Services.GetService<INotificationManagerService>();
                service?.ReceiveNotification(title, message);
            }
        }
    }
}

using Microsoft.Extensions.Logging;

namespace LocalNotificationsDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.Android.NotificationManagerService>();
#elif IOS
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.iOS.NotificationManagerService>();
#elif MACCATALYST
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.MacCatalyst.NotificationManagerService>();
#endif
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}

using Microsoft.Extensions.Logging;
using AgeSignals.Services;

namespace AgeSignals;

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

        // Register Age Signal Service
        #if ANDROID || WINDOWS
        builder.Services.AddSingleton<IAgeSignalService, AgeSignalService>();
        #elif IOS || MACCATALYST
        builder.Services.AddSingleton<IAgeSignalService, Platforms.iOS.AgeSignalService>();
        #else
        builder.Services.AddSingleton<IAgeSignalService, AgeSignalService>();
        #endif

        // Register pages
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

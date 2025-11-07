using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Syncfusion.Maui.Toolkit.Hosting;
using PickerDemo.Control;
using PickerDemo.Handlers;
using Fonts;

namespace PickerDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

        // Register custom handler for Windows, Android, and iOS
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.AddHandler<CustomPicker, CustomPickerHandler>();
#elif WINDOWS
            handlers.AddHandler<CustomPicker, CustomPickerHandler>();
#elif IOS || MACCATALYST
            handlers.AddHandler<CustomPicker, CustomPickerHandler>();
#endif
        });

#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
        });
#endif

        return builder.Build();
    }
}
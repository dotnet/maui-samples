using Microsoft.Extensions.Logging;
using WindowButtonsHandler.Controls;
using WindowButtonsHandler.Handlers;

namespace WindowButtonsHandler
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

        // Register the custom window handler for Mac Catalyst
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<CustomWindow, CustomWindowHandler>();
        });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

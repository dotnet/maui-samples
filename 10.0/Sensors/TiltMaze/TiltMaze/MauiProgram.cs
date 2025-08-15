using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Controls.Handlers;

namespace TiltMaze
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
                })
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler<TiltMaze.EllipseView, GraphicsViewHandler>();
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

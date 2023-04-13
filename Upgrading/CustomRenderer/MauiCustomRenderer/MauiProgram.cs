using XamarinCustomRenderer.Controls;

namespace MauiCustomRenderer;

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
            .ConfigureMauiHandlers((handlers) =>
            {
#if ANDROID
                handlers.AddHandler(typeof(PressableView), typeof(XamarinCustomRenderer.Droid.Renderers.PressableViewRenderer));
#elif IOS
                handlers.AddHandler(typeof(PressableView), typeof(XamarinCustomRenderer.iOS.Renderers.PressableViewRenderer));
#endif
            });

        return builder.Build();
    }
}

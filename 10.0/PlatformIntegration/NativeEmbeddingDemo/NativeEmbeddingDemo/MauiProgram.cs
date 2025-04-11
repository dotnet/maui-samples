using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Embedding;

namespace NativeEmbeddingDemo;

public static class MauiProgram
{
    /// <summary>
    /// Create a new MauiApp using the default application.
    /// </summary>
    /// <param name="additional"></param>
    /// <returns></returns>
    public static MauiApp CreateMauiApp(Action<MauiAppBuilder>? additional = null) =>
        CreateMauiApp<App>(additional);

    /// <summary>
    /// Create a new MauiAPp using the specified application.
    /// </summary>
    /// <typeparam name="TApp"></typeparam>
    /// <param name="additional"></param>
    /// <returns></returns>
    public static MauiApp CreateMauiApp<TApp>(Action<MauiAppBuilder>? additional = null) where TApp : App
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiEmbeddedApp<TApp>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        additional?.Invoke(builder);

        return builder.Build();
    }
}

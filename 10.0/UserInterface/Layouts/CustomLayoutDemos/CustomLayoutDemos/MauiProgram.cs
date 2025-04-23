using Microsoft.Extensions.Logging;

namespace CustomLayoutDemos;

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

        // Setup a custom layout manager so the default manager for the Grid can be replaced.
        builder.Services.Add(new ServiceDescriptor(typeof(ILayoutManagerFactory), new CustomLayoutManagerFactory()));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}


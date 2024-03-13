using Microsoft.Extensions.Logging;

namespace NativeEmbeddingDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp(Action<MauiAppBuilder>? additional = null)
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
            additional?.Invoke(builder);

            return builder.Build();
        }
    }
}

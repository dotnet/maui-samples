using Microsoft.Extensions.Logging;

namespace HybridWebViewDemo
{
    public static class MauiProgram
    {
        static MauiProgram()
        {
            AppContext.SetSwitch("HybridWebView.InvokeJavaScriptThrowsExceptions", isEnabled: true);
        }

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
            builder.Services.AddHybridWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

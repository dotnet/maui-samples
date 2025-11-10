using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleMauiApp.Services;

namespace SampleMauiApp
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

            // Add service defaults & Aspire components.
            builder.AddServiceDefaults();

            // Register services
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();

            // Configure HTTP client for weather API
            builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
                // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
                client.BaseAddress = new("https+http://webapi");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            
            return builder.Build();
        }
    }
}

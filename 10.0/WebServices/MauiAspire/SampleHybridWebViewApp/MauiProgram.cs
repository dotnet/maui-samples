using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace SampleHybridWebViewApp;

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

		// Add service defaults & Aspire components
		builder.AddServiceDefaults();

		// Register the weather proxy service for HybridWebView
		builder.Services.AddSingleton<WeatherProxy>();

		// Configure HTTP client for weather API
		builder.Services.AddHttpClient("weatherapi", client =>
		{
			// This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
			// Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
			client.BaseAddress = new Uri("https+http://webapi");
		});

#if DEBUG
		builder.Services.AddHybridWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

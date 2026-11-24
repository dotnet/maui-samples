using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SampleBlazorHybridApp.Services;

namespace SampleBlazorHybridApp;

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
			});

		// Add service defaults & Aspire components
		builder.AddServiceDefaults();

		// Add Blazor WebView
		builder.Services.AddMauiBlazorWebView();

		// Register services
		builder.Services.AddSingleton<IWeatherService, WeatherService>();

		// Configure HTTP client for weather API
		builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
		{
			// This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
			// Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
			client.BaseAddress = new Uri("https+http://webapi");
		});

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

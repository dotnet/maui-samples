using Microsoft.Extensions.Logging;
using ChatMobile.Client.ViewModels;
using ChatMobile.Client.Views;
using ChatMobile.Client.Services;
using Fonts;
using Syncfusion.Maui.Toolkit.Hosting;

namespace ChatMobile.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureSyncfusionToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
			});

		// Register ViewModels and Pages
		builder.Services.AddSingleton<ChatViewModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<SetupViewModel>();
		builder.Services.AddSingleton<SetupPage>();

		// Services registered in HostingExtensions

		// Configure HttpClient for API communication
		builder.Services.AddHttpClient<IApiService, ApiService>(client =>
		{
			// Use IP address instead of localhost to avoid DNS issues
			client.BaseAddress = new Uri("http://127.0.0.1:5132/");
			client.Timeout = TimeSpan.FromSeconds(30);
		});

		// Register AI chat client with tools
		builder.AddChatClientWithTools();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

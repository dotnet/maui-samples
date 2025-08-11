using Microsoft.Extensions.Logging;
using SimpleChatClient.ViewModels;
using SimpleChatClient.Services;
using Fonts;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SimpleChatClient;

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

		// Register ViewModels
		builder.Services.AddSingleton<ChatViewModel>();

		// Register AI chat client (Foundry/OpenAI) from environment variables (desktop only)
#if WINDOWS || MACCATALYST
		builder.AddFoundryChatClient();
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

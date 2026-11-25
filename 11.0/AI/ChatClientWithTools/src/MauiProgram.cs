using Microsoft.Extensions.Logging;
using ChatClientWithTools.ViewModels;
using ChatClientWithTools.Services;
using Fonts;
using Syncfusion.Maui.Toolkit.Hosting;

namespace ChatClientWithTools;

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

		// Register AI chat client with tools (desktop only)
#if WINDOWS || MACCATALYST
		builder.AddChatClientWithTools();
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

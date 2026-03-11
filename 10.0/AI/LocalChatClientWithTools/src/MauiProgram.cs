using Microsoft.Extensions.Logging;
using LocalChatClientWithTools.ViewModels;
using LocalChatClientWithTools.Services;
using Fonts;
using Syncfusion.Maui.Toolkit.Hosting;

namespace LocalChatClientWithTools;

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

		// Register AI chat client with tools (Apple Intelligence — iOS/macCatalyst only)
#if IOS || MACCATALYST
		builder.AddLocalChatClientWithTools();
#else
		throw new PlatformNotSupportedException(
			"This sample requires Apple Intelligence and only runs on iOS 26+ or macCatalyst 26+.");
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

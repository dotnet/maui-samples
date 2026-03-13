using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using LocalChatClientWithTools.ViewModels;
using LocalChatClientWithTools.Services.Tools;
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

        // Register tool services
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<CalculatorTool>();
        builder.Services.AddSingleton<WeatherTool>();
        builder.Services.AddSingleton<FileOperationsTool>();
        builder.Services.AddSingleton<SystemInfoTool>();
        builder.Services.AddSingleton<TimerTool>();

        // Register AI tool functions
        builder.Services.AddSingleton(CalculatorTool.CreateAIFunction);
        builder.Services.AddSingleton(WeatherTool.CreateAIFunction);
        builder.Services.AddSingleton(FileOperationsTool.CreateAIFunction);
        builder.Services.AddSingleton(SystemInfoTool.CreateAIFunction);
        builder.Services.AddSingleton(TimerTool.CreateAIFunction);

#if IOS || MACCATALYST
#pragma warning disable CA1416
        builder.Services.AddSingleton<Microsoft.Maui.Essentials.AI.AppleIntelligenceChatClient>();

        builder.Services.AddChatClient(sp =>
        {
            var appleClient = sp.GetRequiredService<Microsoft.Maui.Essentials.AI.AppleIntelligenceChatClient>();
            return new ChatClientBuilder(appleClient)
                .UseFunctionInvocation()
                .UseLogging()
                .Build(sp);
        });
#pragma warning restore CA1416
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

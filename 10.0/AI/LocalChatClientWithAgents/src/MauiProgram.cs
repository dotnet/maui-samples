using LocalChatClientWithAgents.AI;
using LocalChatClientWithAgents.Pages;
using LocalChatClientWithAgents.Services;
using LocalChatClientWithAgents.ViewModels;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Essentials.AI;

namespace LocalChatClientWithAgents;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder.UseMauiApp<App>();

#if IOS || ANDROID || MACCATALYST
		builder.UseMauiMaps();
#endif

		builder.ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		});

		// Register AI agents and workflow (Apple Intelligence only)
#if IOS || MACCATALYST
		builder.AddAppleIntelligenceServices();
#else
		throw new PlatformNotSupportedException(
			"This sample requires Apple Intelligence and is only supported on iOS and macCatalyst.");
#endif
		builder.AddItineraryWorkflow();

		// Register Pages
		builder.Services.AddTransient<LandmarksPage>();
		builder.Services.AddTransient<TripPlanningPage>();

		// Register ViewModels
		builder.Services.AddTransient<LandmarksViewModel>();
		builder.Services.AddTransient<TripPlanningViewModel>();
		builder.Services.AddSingleton<ChatViewModel>();

		// Register Services
		builder.Services.AddSingleton<DataService>();
		builder.Services.AddSingleton<LanguagePreferenceService>();
		builder.Services.AddTransient<ItineraryService>();
		builder.Services.AddTransient<TaggingService>();
		builder.Services.AddHttpClient<WeatherService>();
		builder.Services.AddSingleton<ChatService>();

		// Configure Logging
		builder.Services.AddLogging();
		builder.Logging.AddDebug();
		builder.Logging.AddConsole();
#if DEBUG
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
		builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

		return builder.Build();
	}

#if IOS || MACCATALYST
#pragma warning disable CA1416 // Validate platform compatibility - this sample requires iOS/macCatalyst 26.0+
	private static MauiAppBuilder AddAppleIntelligenceServices(this MauiAppBuilder builder)
	{
		// Register the base Apple Intelligence client
		builder.Services.AddSingleton<AppleIntelligenceChatClient>();

		// Register the Apple Intelligence client as IChatClient to allow direct use
		builder.Services.AddSingleton<IChatClient>(sp =>
		{
			var appleClient = sp.GetRequiredService<AppleIntelligenceChatClient>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return appleClient
				.AsBuilder()
				.UseLogging(loggerFactory)
				.Build();
		});

		// Register the Agent Framework wrapper as "local-model"
		builder.Services.AddKeyedSingleton<IChatClient>("local-model", (sp, _) =>
		{
			var appleClient = sp.GetRequiredService<AppleIntelligenceChatClient>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return appleClient
				.AsBuilder()
				.UseLogging(loggerFactory)
				.Build();
		});

		// Register "cloud-model" with buffering
		builder.Services.AddKeyedSingleton<IChatClient>("cloud-model", (sp, _) =>
		{
			var appleClient = sp.GetRequiredService<AppleIntelligenceChatClient>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return appleClient
				.AsBuilder()
				.UseLogging(loggerFactory)
				.Use(cc => new BufferedChatClient(cc))
				.Build();
		});

		// Register the Natural Language Embedding generator
		builder.Services.AddSingleton<NLEmbeddingGenerator>();

		// Register embedding generator
		builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
		{
			var embeddings = sp.GetRequiredService<NLEmbeddingGenerator>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return embeddings.AsBuilder()
				.UseLogging(loggerFactory)
				.Build();
		});

		return builder;
	}
#pragma warning restore CA1416
#endif
}

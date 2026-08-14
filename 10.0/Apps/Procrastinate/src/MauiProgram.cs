using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using procrastinate.Pages;
using procrastinate.Services;

namespace procrastinate;

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
				fonts.AddFont("FontAwesome-Solid.otf", "FontAwesomeSolid");
			});

		// Register Apple Intelligence chat client and NLEmbedding (iOS/macOS only)
#if IOS || MACCATALYST
#pragma warning disable CA1416, MAUIAI0001
		try
		{
			var appleClient = new Microsoft.Maui.Essentials.AI.AppleIntelligenceChatClient();
			builder.Services.AddSingleton<IChatClient>(appleClient);
			System.Diagnostics.Debug.WriteLine("Apple Intelligence chat client registered");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Apple Intelligence not available: {ex.Message}");
		}

		try
		{
			var nlEmbedding = new Microsoft.Maui.Essentials.AI.NLEmbeddingGenerator();
			builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(nlEmbedding);
			System.Diagnostics.Debug.WriteLine("NLEmbedding generator registered");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"NLEmbedding not available: {ex.Message}");
		}
#pragma warning restore CA1416, MAUIAI0001
#endif

		// Services
		builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddSingleton<StatsService>();
		builder.Services.AddSingleton<ExcuseService>();
		builder.Services.AddSingleton<TicTacToeAI>();
		
		// Pages
		builder.Services.AddTransient<TasksPage>();
		builder.Services.AddTransient<GamesPage>();
		builder.Services.AddTransient<ExcusePage>();
		builder.Services.AddTransient<StatsPage>();
		builder.Services.AddTransient<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

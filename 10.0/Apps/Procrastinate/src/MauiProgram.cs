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

		// Services
		builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddSingleton<StatsService>();
		builder.Services.AddSingleton<ExcuseService>();
		
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

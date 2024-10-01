using Microsoft.Extensions.Logging;

namespace PlatformIntegrationDemo;

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
			})
			.ConfigureEssentials(essentials =>
			{
				essentials.UseVersionTracking();
                essentials.AddAppAction("app_info", "App Info", icon: "app_info_action_icon");
                essentials.AddAppAction("battery_info", "Battery Info");
                essentials.OnAppAction(App.HandleAppActions);
#if WINDOWS
				essentials.UseMapServiceToken("INSERT_MAP_TOKEN_HERE");
#endif
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}


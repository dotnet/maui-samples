using Microsoft.Extensions.Logging;
using Microsoft.Maui.Foldable;

namespace MauiTwoPaneViewDemo;

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
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Adapt to dual-screen and foldable Android devices like Surface Duo, includes TwoPaneView layout control
        builder.UseFoldable();

        return builder.Build();
	}
}

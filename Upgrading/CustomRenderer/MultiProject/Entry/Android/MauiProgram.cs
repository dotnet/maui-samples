using Microsoft.Maui.Hosting;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace CustomRenderer.Android;

public static class MauiProgram
{
	public static MauiAppBuilder builder;
        public static MauiAppBuilder Builder
        {
            get
            {
                if (builder == null)
                {
                    builder = MauiApp.CreateBuilder();
                }

                return builder;
            }
        }
	public static MauiApp CreateMauiApp()
	{
		var builder = Builder;
		builder
			.UseMauiCompatibility()
			.UseMauiApp<App>();

		builder.ConfigureMauiHandlers(handlers => {
#if ANDROID
			handlers.AddCompatibilityRenderer(typeof(MyEntry), typeof(CustomRenderer.Android.MyEntryRenderer));	
#endif
		});

		return builder.Build();
	}
}

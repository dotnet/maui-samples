using Microsoft.Maui.Hosting;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Hosting;

namespace CustomRenderer.iOS;

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
			handlers.AddCompatibilityRenderer(typeof(CustomRenderer.MyEntry), typeof(CustomRenderer.iOS.MyEntryRenderer));	
		});

		return builder.Build();
	}
}

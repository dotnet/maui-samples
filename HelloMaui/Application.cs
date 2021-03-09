using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;

namespace HelloMaui
{
	public class Application : MauiApp
	{
		public override IAppHostBuilder CreateBuilder() => 
			base.CreateBuilder()
				.RegisterCompatibilityRenderers()
				.ConfigureServices((ctx, services) =>
				{
					services.AddTransient<MainPage>();
					services.AddTransient<IWindow, MainWindow>();
				})
				.ConfigureFonts((hostingContext, fonts) =>
				{
					fonts.AddFont("ionicons.ttf", "IonIcons");
				});

		public override IWindow CreateWindow(IActivationState state)
		{
			Microsoft.Maui.Controls.Compatibility.Forms.Init(state);
			return Services.GetService<IWindow>();
		}
	}
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;

namespace HelloMaui
{
	public class Startup : IStartup
	{
		public readonly static bool UseXamlPage = false;
		public readonly static bool UseXamlApp = true;

		public void Configure(IAppHostBuilder appBuilder)
		{
			appBuilder = 
				appBuilder
					.UseFormsCompatibility()
					.UseMauiApp<Application>();

			appBuilder
				.UseMauiServiceProviderFactory(true)
				.ConfigureServices(services =>
				{
					services.AddTransient<IPage, MainPage>();
					services.AddTransient<IWindow, MainWindow>();
				})
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("ionicons.ttf", "IonIcons");
				});
		}
	}
}
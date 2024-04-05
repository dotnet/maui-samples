using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace CustomRenderer;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiCompatibility()
			.UseMauiApp<App>();

		return builder.Build();
	}
}

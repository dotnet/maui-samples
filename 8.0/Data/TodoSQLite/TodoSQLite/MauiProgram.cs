using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoSQLite.Data;
using TodoSQLite.Views;

namespace TodoSQLite;

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

		builder.Services.AddSingleton<TodoListPage>();
		builder.Services.AddTransient<TodoItemPage>();

		builder.Services.AddSingleton<TodoItemDatabase>();

		return builder.Build();
	}
}

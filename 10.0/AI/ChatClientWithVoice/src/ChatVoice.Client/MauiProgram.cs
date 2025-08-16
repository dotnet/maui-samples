using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Fonts;
using ChatVoice.Client.Views;
using ChatVoice.Client.ViewModels;
using ChatVoice.Client.Services;
using Syncfusion.Maui.Toolkit.Hosting;

namespace ChatVoice.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

        // Register ViewModels and Pages
        builder.Services.AddSingleton<ChatViewModel>();
        builder.Services.AddSingleton<SetupViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SetupPage>();

        // Configure HttpClient for API communication
        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri("http://127.0.0.1:5132/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Services
        builder.Services.AddSingleton<ISecureCredentialService, SecureCredentialService>();
        builder.AddChatClientWithTools();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }
}

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LocalChatClientWithTools.Services.Tools;

namespace LocalChatClientWithTools.Services;

public static class HostingExtensions
{
    public static MauiAppBuilder AddLocalChatClientWithTools(this MauiAppBuilder builder)
    {
        // HttpClient for Weather API (open-meteo.com, no API key required)
        builder.Services.AddHttpClient<WeatherTool>();

        // Register tools used in the sample
        builder.Services.AddSingleton<CalculatorTool>();
        builder.Services.AddSingleton<WeatherTool>();
        builder.Services.AddSingleton<FileOperationsTool>();
        builder.Services.AddSingleton<SystemInfoTool>();
        builder.Services.AddSingleton<TimerTool>();

#if IOS || MACCATALYST
#pragma warning disable CA1416 // Validate platform compatibility - requires iOS/macCatalyst 26.0+
        builder.Services.AddSingleton<Microsoft.Maui.Essentials.AI.AppleIntelligenceChatClient>();

        builder.Services.AddChatClient(sp =>
        {
            var appleClient = sp.GetRequiredService<Microsoft.Maui.Essentials.AI.AppleIntelligenceChatClient>();
            return appleClient.AsBuilder()
                .UseFunctionInvocation()
                .UseLogging()
                .Build(sp);
        });
#pragma warning restore CA1416
#endif

        return builder;
    }
}
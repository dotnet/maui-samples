using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChatMobile.Client.Tools;

namespace ChatMobile.Client.Services;

public static class HostingExtensions
{
    public static MauiAppBuilder AddChatClientWithTools(this MauiAppBuilder builder)
    {
        // Register secure credential service
        builder.Services.AddSingleton<ISecureCredentialService, SecureCredentialService>();

        // Register chat service
        builder.Services.AddScoped<IChatService, ChatService>();

        // LocalToolService removed; tools are provided directly via ChatOptions

        // HttpClient for Weather API
        builder.Services.AddHttpClient<WeatherTool>();

        // Register tools used in the sample (calculator can be created inline as well)
        builder.Services.AddSingleton<CalculatorTool>();

        // Don't register IChatClient directly - ChatService will create it dynamically

        return builder;
    }
}


using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChatClientWithTools.Services.Tools;

namespace ChatClientWithTools.Services;

public static class HostingExtensions
{
    public static MauiAppBuilder AddChatClientWithTools(this MauiAppBuilder builder)
    {
        // HttpClient for Weather API
        builder.Services.AddHttpClient<WeatherTool>();

        // Register tools used in the sample
        builder.Services.AddSingleton<CalculatorTool>();
        builder.Services.AddSingleton<WeatherTool>();
        builder.Services.AddSingleton<FileOperationsTool>();
        builder.Services.AddSingleton<SystemInfoTool>();
        builder.Services.AddSingleton<TimerTool>();

        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("ChatClientWithTools");
            var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            var model = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL") ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Missing AZURE_OPENAI_ENDPOINT or AZURE_OPENAI_API_KEY environment variables.");
            }

            logger.LogInformation("Initializing Azure OpenAI chat client (model: {Model})", model);
            var aoai = new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey));

            var client = new ChatClientBuilder(aoai.GetChatClient(model).AsIChatClient())
                                .UseFunctionInvocation()
                                .Build();

            return client;
        });

        return builder;
    }
}
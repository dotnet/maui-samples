using System;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleChatClient.Services;

public static class HostingExtensions
{
    // Registers a singleton IChatClient backed by Azure AI Foundry/OpenAI-compatible endpoint.
    // Desktop dev: reads AZURE_OPENAI_ENDPOINT, AZURE_OPENAI_API_KEY, optional AZURE_OPENAI_MODEL (defaults gpt-4o-mini).
    public static MauiAppBuilder AddFoundryChatClient(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("FoundryChatClient");

            var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            var model = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL");

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Missing AZURE_OPENAI_ENDPOINT or AZURE_OPENAI_API_KEY environment variables.");
            }

            logger.LogInformation("Initializing Azure OpenAI (Foundry) chat client for model {Model} at {Endpoint}", model, endpoint);

            // For Azure AI Foundry, use AzureOpenAIClient with API key credential.
            var aoai = new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey));

            // Microsoft.Extensions.AI.OpenAI exposes AsIChatClient() to adapt to IChatClient
            var chat = aoai.GetChatClient(model).AsIChatClient();

            return chat;
        });

        return builder;
    }
}

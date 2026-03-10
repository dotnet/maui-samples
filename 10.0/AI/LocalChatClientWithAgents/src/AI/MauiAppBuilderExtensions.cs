using LocalChatClientWithAgents.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Essentials.AI;

namespace LocalChatClientWithAgents.AI;

public static class MauiAppBuilderExtensions
{
#if IOS || MACCATALYST
#pragma warning disable CA1416 // Validate platform compatibility - this sample requires iOS/macCatalyst 26.0+
	public static MauiAppBuilder AddAppleIntelligenceServices(this MauiAppBuilder builder)
	{
		// Register the base Apple Intelligence client
		builder.Services.AddSingleton<AppleIntelligenceChatClient>();

		// Register the Apple Intelligence client as IChatClient to allow direct use
		builder.Services.AddSingleton<IChatClient>(sp =>
		{
			var appleClient = sp.GetRequiredService<AppleIntelligenceChatClient>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return appleClient
				.AsBuilder()
				.UseLogging(loggerFactory)
				.Build();
		});

		// Register the Agent Framework wrapper as "local-model"
		builder.Services.AddKeyedSingleton<IChatClient>("local-model", (sp, _) =>
		{
			var appleClient = sp.GetRequiredService<AppleIntelligenceChatClient>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return appleClient
				.AsBuilder()
				.UseLogging(loggerFactory)
				.Build();
		});

		// Register "cloud-model" with buffering
		builder.Services.AddKeyedSingleton<IChatClient>("cloud-model", (sp, _) =>
		{
			var appleClient = sp.GetRequiredService<AppleIntelligenceChatClient>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return appleClient
				.AsBuilder()
				.UseLogging(loggerFactory)
				.Use(cc => new BufferedChatClient(cc))
				.Build();
		});

		// Register the Natural Language Embedding generator
		builder.Services.AddSingleton<NLEmbeddingGenerator>();

		// Register embedding generator
		builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
		{
			var embeddings = sp.GetRequiredService<NLEmbeddingGenerator>();
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return embeddings.AsBuilder()
				.UseLogging(loggerFactory)
				.Build();
		});

		return builder;
	}
#pragma warning restore CA1416
#endif
}

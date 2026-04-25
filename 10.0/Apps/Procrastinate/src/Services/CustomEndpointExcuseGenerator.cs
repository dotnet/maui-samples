using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// "Bring Your Own Model" excuse generator.
/// Connects to any OpenAI-compatible endpoint configured in Settings.
/// </summary>
public class CustomEndpointExcuseGenerator : IExcuseGenerator
{
    public string Name => "Custom AI (BYOM)";
    public bool IsAvailable => !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(Model);

    private static string Endpoint => Preferences.Get("CustomAIEndpoint", "");
    private static string ApiKey => SecureStorage.GetAsync("CustomAIApiKey").GetAwaiter().GetResult() ?? "";
    private static string Model => Preferences.Get("CustomAIModel", "");

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!IsAvailable)
            return new ExcuseResult("Configure a custom endpoint in Settings.", Name, stopwatch.Elapsed);

        try
        {
            var languageName = language switch
            {
                "fr" => "French",
                "es" => "Spanish",
                "pt" => "Portuguese",
                "nl" => "Dutch",
                "cs" => "Czech",
                "uk" => "Ukrainian",
                _ => "English"
            };

            var topics = new[]
            {
                "a pet", "aliens", "time travel", "a haunted toaster", "quantum physics",
                "a philosophical duck", "a sentient roomba", "moon phases", "a ninja hamster",
                "a glitch in the matrix", "a cursed elevator", "a runaway flamingo"
            };
            var topic = topics[Random.Shared.Next(topics.Length)];

            var prompt = $"Generate a single funny, creative excuse about {topic} for not doing work. Write it in {languageName}. Reply with ONLY the excuse text.";

            using var chatClient = CreateChatClient();
            var messages = new List<ChatMessage> { new(ChatRole.User, prompt) };

            var response = await chatClient.GetResponseAsync(messages);
            stopwatch.Stop();

            var excuse = response.Text?.Trim() ?? "The custom AI is also procrastinating...";
            return new ExcuseResult(excuse, Name, stopwatch.Elapsed, Model: Model);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"Custom AI error: {ex}");
            return new ExcuseResult("The custom model tripped over its own neurons. Check the endpoint and try again! 🧠", Name, stopwatch.Elapsed, Model: Model);
        }
    }

    private static IChatClient CreateChatClient()
    {
        var credential = string.IsNullOrEmpty(ApiKey)
            ? new System.ClientModel.ApiKeyCredential("no-key")
            : new System.ClientModel.ApiKeyCredential(ApiKey);

        return new OpenAI.OpenAIClient(
                credential,
                new OpenAI.OpenAIClientOptions { Endpoint = new Uri(Endpoint) })
            .GetChatClient(Model)
            .AsIChatClient();
    }
}

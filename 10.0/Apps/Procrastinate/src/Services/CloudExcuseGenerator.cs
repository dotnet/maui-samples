using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

public class CloudExcuseGenerator : IExcuseGenerator
{
    public string Name => "Cloud AI (Groq)";
    public bool IsAvailable => IsCloudAvailable;

    public static bool IsCloudAvailable => !string.IsNullOrEmpty(GetApiKey());

    internal static string GetApiKey() => SecureStorage.GetAsync("GroqApiKey").GetAwaiter().GetResult() ?? "";
    private static string ApiEndpoint => Preferences.Get("GroqApiEndpoint", "https://api.groq.com/openai/v1");
    private static string Model => Preferences.Get("GroqModel", "llama-3.3-70b-versatile");

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!IsAvailable)
            return new ExcuseResult("Cloud AI requires an API key. Configure it in Settings.", Name, stopwatch.Elapsed);

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

            var prompt = $"Generate a single funny, creative excuse for not doing work or being productive. The excuse should be absurd but delivered with a straight face. Write it in {languageName}. Reply with ONLY the excuse text, no quotes or explanation.";

            using var chatClient = CreateChatClient();
            var messages = new List<ChatMessage>
            {
                new(ChatRole.User, prompt)
            };

            var response = await chatClient.GetResponseAsync(messages);
            stopwatch.Stop();

            var excuse = response.Text?.Trim() ?? "The AI is also procrastinating...";

            return new ExcuseResult(excuse, Name, stopwatch.Elapsed, Model: Model);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"Cloud excuse error: {ex}");
            return new ExcuseResult("The cloud AI is napping. Try again or switch to on-device! ☁️💤", Name, stopwatch.Elapsed, Model: Model);
        }
    }

    internal static IChatClient CreateChatClient()
    {
        return new OpenAI.OpenAIClient(
                new System.ClientModel.ApiKeyCredential(GetApiKey()),
                new OpenAI.OpenAIClientOptions { Endpoint = new Uri(ApiEndpoint) })
            .GetChatClient(Model)
            .AsIChatClient();
    }
}

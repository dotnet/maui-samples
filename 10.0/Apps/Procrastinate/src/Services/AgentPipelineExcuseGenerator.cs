using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// Multi-agent excuse pipeline: Researcher → Writer → Editor.
/// Each agent is a separate IChatClient call, chained sequentially.
/// Reports progress via the OnStageChanged callback.
/// </summary>
public class AgentPipelineExcuseGenerator : IExcuseGenerator
{
    private readonly IChatClient? _onDeviceChatClient;

    public string Name => "AI Agent Pipeline";
    public bool IsAvailable => _onDeviceChatClient is not null || CloudExcuseGenerator.IsCloudAvailable;

    /// <summary>
    /// Callback fired when the pipeline moves to a new stage.
    /// </summary>
    public Action<string>? OnStageChanged { get; set; }

    /// <summary>
    /// Callback fired when an agent produces output. (stageName, agentOutput)
    /// </summary>
    public Action<string, string>? OnAgentOutput { get; set; }

    public AgentPipelineExcuseGenerator(IChatClient? onDeviceChatClient = null)
    {
        _onDeviceChatClient = onDeviceChatClient;
    }

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        IChatClient? client = _onDeviceChatClient;
        IChatClient? disposableClient = null;
        var modelName = "Apple Intelligence";

        if (client is null && CloudExcuseGenerator.IsCloudAvailable)
        {
            disposableClient = CloudExcuseGenerator.CreateChatClient();
            client = disposableClient;
            modelName = "Cloud AI";
        }

        if (client is null)
        {
            stopwatch.Stop();
            return new ExcuseResult("No AI available. Configure Cloud AI or use a device with Apple Intelligence.", Name, stopwatch.Elapsed);
        }

        try
        {
            var languageName = GetLanguageName(language);

            // Agent 1: Researcher — picks an absurd scenario
            OnStageChanged?.Invoke("🔍 Agent 1: Researching...");
            var scenario = await RunAgentAsync(client,
                "You are a creative comedy researcher. Your job is to come up with an absurd, unexpected scenario that could be used as an excuse. Output ONLY the scenario in 1-2 sentences. Be wildly creative.",
                $"Come up with a bizarre, funny scenario involving {GetRandomElement()}. Make it unexpected and absurd. Just the scenario, nothing else.");
            OnAgentOutput?.Invoke("🔍 Researcher", scenario);

            // Agent 2: Writer — crafts the excuse from the scenario
            OnStageChanged?.Invoke("✍️ Agent 2: Writing...");
            var rawExcuse = await RunAgentAsync(client,
                "You are a comedy writer. You turn scenarios into first-person excuses that sound like something a real person would say. Keep it to 1-2 sentences. Start with 'I' or 'Sorry'.",
                $"Turn this scenario into a funny first-person excuse in {languageName}:\n\n{scenario}\n\nJust the excuse, nothing else.");
            OnAgentOutput?.Invoke("✍️ Writer", rawExcuse);

            // Agent 3: Editor — polishes and ensures quality
            OnStageChanged?.Invoke("✨ Agent 3: Polishing...");
            var finalExcuse = await RunAgentAsync(client,
                $"You are a comedy editor. You polish excuses to be funnier and more natural-sounding. Keep the same language ({languageName}). Output ONLY the polished excuse.",
                $"Polish this excuse to be funnier and more natural. Keep it in {languageName}. Keep it to 1-2 sentences:\n\n{rawExcuse}\n\nJust the polished excuse, nothing else.");
            OnAgentOutput?.Invoke("✨ Editor", finalExcuse.Trim());

            OnStageChanged?.Invoke("✅ Done!");
            stopwatch.Stop();

            return new ExcuseResult(
                finalExcuse.Trim(),
                Name,
                stopwatch.Elapsed,
                Model: $"{modelName} (3 agents)");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"Pipeline error: {ex}");

            var friendly = ex.Message switch
            {
                string m when m.Contains("content", StringComparison.OrdinalIgnoreCase) &&
                              m.Contains("unsafe", StringComparison.OrdinalIgnoreCase)
                    => "The AI thought that excuse was too spicy! Try again for a tamer one. 🌶️",
                string m when m.Contains("content", StringComparison.OrdinalIgnoreCase) &&
                              m.Contains("filter", StringComparison.OrdinalIgnoreCase)
                    => "The AI's content filter kicked in — apparently that excuse was TOO creative. Try again! 🎨",
                string m when m.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                              m.Contains("timed out", StringComparison.OrdinalIgnoreCase)
                    => "Even the AI is procrastinating! It took too long. Try again. ⏰",
                string m when m.Contains("network", StringComparison.OrdinalIgnoreCase) ||
                              m.Contains("connection", StringComparison.OrdinalIgnoreCase)
                    => "No connection — the AI agents are on a coffee break. Check your network. ☕",
                string m when m.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                              m.Contains("429", StringComparison.OrdinalIgnoreCase)
                    => "Too many excuses requested! The AI needs a breather. Wait a moment. 😮‍💨",
                _ => "The excuse factory had a hiccup. Give it another shot! 🏭"
            };

            return new ExcuseResult(friendly, Name, stopwatch.Elapsed);
        }
        finally
        {
            (disposableClient as IDisposable)?.Dispose();
        }
    }

    private static async Task<string> RunAgentAsync(IChatClient client, string systemPrompt, string userPrompt)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt)
        };

        var response = await client.GetResponseAsync(messages);
        return response.Text?.Trim() ?? "";
    }

    private static string GetRandomElement()
    {
        var elements = new[]
        {
            "a time-traveling pigeon", "sentient office furniture", "a secret society of squirrels",
            "a parallel universe where gravity is optional", "a conspiracy involving socks",
            "an AI that became a life coach", "a haunted coffee machine", "quantum entangled twins",
            "a diplomatic incident with a penguin", "a rogue weather satellite",
            "an enchanted parking meter", "a philosophical debate with a cat",
            "a mysterious portal in the closet", "an accidental invention", "a cursed alarm clock",
            "a runaway sourdough starter", "an overly helpful robot vacuum",
            "a neighborhood raccoon uprising", "a telepathic houseplant", "a glitch in spacetime"
        };
        return elements[Random.Shared.Next(elements.Length)];
    }

    private static string GetLanguageName(string language) => language switch
    {
        "fr" => "French",
        "es" => "Spanish",
        "pt" => "Portuguese",
        "nl" => "Dutch",
        "cs" => "Czech",
        "uk" => "Ukrainian",
        _ => "English"
    };
}

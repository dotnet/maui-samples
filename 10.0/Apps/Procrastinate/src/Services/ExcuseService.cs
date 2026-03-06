using Microsoft.Extensions.AI;

namespace procrastinate.Services;

public class ExcuseService
{
    private readonly StatsService _stats;
    private readonly IChatClient? _onDeviceChatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>>? _embeddingGenerator;
    private readonly RandomExcuseGenerator _randomGenerator = new();

    /// <summary>
    /// Callback for pipeline stage updates (used by Agent Pipeline mode).
    /// </summary>
    public Action<string>? OnPipelineStageChanged { get; set; }

    /// <summary>
    /// Callback for agent reasoning output (stageName, agentOutput).
    /// </summary>
    public Action<string, string>? OnAgentOutput { get; set; }

    public ExcuseService(
        StatsService stats,
        IChatClient? onDeviceChatClient = null,
        IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator = null)
    {
        _stats = stats;
        _onDeviceChatClient = onDeviceChatClient;
        _embeddingGenerator = embeddingGenerator;
    }

    public static string CurrentMode
    {
        get => Preferences.Get("ExcuseMode", "random");
        set => Preferences.Set("ExcuseMode", value);
    }

    public static Dictionary<string, string> AvailableModes
    {
        get
        {
            var modes = new Dictionary<string, string>
            {
                { "random", "Random Generator" },
                { "cloud", "Cloud AI (MEAI + Groq)" }
            };

#if IOS || MACCATALYST
            modes.Add("ondevice", "On-Device AI (MEAI + Apple Intelligence)");
#endif
            modes.Add("pipeline", "🤖 AI Agent Pipeline (3 agents)");
            modes.Add("custom", "Custom Endpoint (BYOM)");
            modes.Add("embedded", "📦 Embedded ONNX Model (offline)");
            return modes;
        }
    }

    /// <summary>
    /// Returns true if on-device AI (Apple Intelligence via MEAI) is available.
    /// </summary>
    public bool IsOnDeviceAvailable => _onDeviceChatClient is not null;

    /// <summary>
    /// Returns true if NLEmbedding quality scoring is available.
    /// </summary>
    public bool IsEmbeddingAvailable => _embeddingGenerator is not null;

    public IExcuseGenerator GetCurrentGenerator()
    {
        return CurrentMode switch
        {
            "cloud" => new CloudExcuseGenerator(),
            "ondevice" => new OnDeviceAIExcuseGenerator(_onDeviceChatClient),
            "pipeline" => new AgentPipelineExcuseGenerator(_onDeviceChatClient) { OnStageChanged = OnPipelineStageChanged, OnAgentOutput = OnAgentOutput },
            "custom" => new CustomEndpointExcuseGenerator(),
            "embedded" => new EmbeddedModelExcuseGenerator(),
            _ => _randomGenerator
        };
    }

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var generator = GetCurrentGenerator();
        var usingAI = CurrentMode != "random" && generator.IsAvailable;

        var result = await generator.GenerateExcuseAsync(language);

        // Quality scoring with NLEmbedding (when available and using AI)
        if (usingAI && _embeddingGenerator is not null && !result.Excuse.StartsWith("Error") && !result.Excuse.Contains("error"))
        {
            var score = await ExcuseQualityScorer.ScoreAsync(_embeddingGenerator, result.Excuse);

            // If quality is low, retry once with a fresh prompt
            if (score < 0.3f)
            {
                OnPipelineStageChanged?.Invoke("🔄 Low quality, retrying...");
                var retry = await generator.GenerateExcuseAsync(language);
                var retryScore = await ExcuseQualityScorer.ScoreAsync(_embeddingGenerator, retry.Excuse);

                // Use whichever scored better
                if (retryScore > score)
                {
                    result = retry with { Model = result.Model + " (retry)" };
                    score = retryScore;
                }
            }

            // Append quality score to model info
            result = result with { Model = $"{result.Model} · quality:{score:F2}" };
        }

        if (usingAI)
        {
            _stats.IncrementAIExcuseCalls();
        }

        return result;
    }
}

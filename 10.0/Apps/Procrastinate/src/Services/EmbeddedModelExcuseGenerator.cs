using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// Generates excuses using an embedded ONNX model running entirely on-device.
/// On iOS, this is not available (use Apple Intelligence instead); works on Android/macCatalyst.
/// </summary>
public class EmbeddedModelExcuseGenerator : IExcuseGenerator
{
    private readonly string _modelId;

    public string Name => "Embedded ONNX AI";

    public EmbeddedModelExcuseGenerator(string modelId = "phi3-mini-int4")
    {
        _modelId = modelId;
    }

#if IOS
    public bool IsAvailable => false;

    public Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        return Task.FromResult(new ExcuseResult(
            "Embedded ONNX model is not available on iOS. Use Apple Intelligence instead!",
            Name, TimeSpan.Zero));
    }
#else
    public bool IsAvailable => OnnxModelManager.IsModelDownloaded(_modelId);

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!IsAvailable)
        {
            stopwatch.Stop();
            return new ExcuseResult(
                "Model not downloaded. Go to Settings to download the embedded model.",
                Name, stopwatch.Elapsed);
        }

        var modelPath = OnnxModelManager.GetModelDirectory(_modelId);
        var modelInfo = OnnxModelManager.AvailableModels.First(m => m.Id == _modelId);

        try
        {
            var client = OnnxGenAIChatClient.GetOrCreate(modelPath, modelInfo.Name);

            var topic = OnDeviceAIExcuseGenerator.RandomTopics[
                Random.Shared.Next(OnDeviceAIExcuseGenerator.RandomTopics.Length)];
            var starter = OnDeviceAIExcuseGenerator.StarterPhrases[
                Random.Shared.Next(OnDeviceAIExcuseGenerator.StarterPhrases.Length)];

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, $"You are a creative excuse generator. Generate ONE short, funny, creative excuse for procrastinating. " +
                    $"The excuse should be about: {topic}. " +
                    $"Start with: \"{starter}\". " +
                    $"Language: {language}. " +
                    $"Be witty, absurd, and under 2 sentences. Do not repeat the prompt."),
                new(ChatRole.User, $"Give me a funny excuse for procrastinating about {topic}.")
            };

            var response = await client.GetResponseAsync(messages, new ChatOptions
            {
                MaxOutputTokens = 150,
                Temperature = 0.9f
            });

            stopwatch.Stop();
            var excuse = response.Text?.Trim() ?? "The ONNX model contemplated too deeply and forgot to respond.";

            return new ExcuseResult(excuse, Name, stopwatch.Elapsed, Model: modelInfo.Name);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Debug.WriteLine($"Embedded model error: {ex}");
            return new ExcuseResult($"Error running embedded model: {ex.Message}", Name, stopwatch.Elapsed);
        }
    }
#endif
}

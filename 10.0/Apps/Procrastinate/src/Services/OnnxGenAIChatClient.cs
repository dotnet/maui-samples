#if !IOS
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.AI;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace procrastinate.Services;

/// <summary>
/// IChatClient wrapper around ONNX Runtime GenAI for embedded local model inference.
/// Available on Android and macCatalyst (excluded from iOS due to native linker incompatibility).
/// </summary>
public class OnnxGenAIChatClient : IChatClient
{
    private readonly Model _model;
    private readonly Tokenizer _tokenizer;
    private bool _disposed;

    public ChatClientMetadata Metadata { get; }

    private static readonly object _lock = new();
    private static OnnxGenAIChatClient? _cached;
    private static string? _cachedPath;
    private static int _activeInferences;

    public static OnnxGenAIChatClient GetOrCreate(string modelPath, string modelName = "ONNX Local")
    {
        lock (_lock)
        {
            if (_cached != null && !_cached._disposed && _cachedPath == modelPath)
                return _cached;

            if (_cached != null && _activeInferences == 0)
            {
                _cached.DisposeInternal();
            }

            _cached = new OnnxGenAIChatClient(modelPath, modelName);
            _cachedPath = modelPath;
            return _cached;
        }
    }

    public static void UnloadCached()
    {
        lock (_lock)
        {
            if (_cached != null && _activeInferences == 0)
            {
                _cached.DisposeInternal();
            }
            _cached = null;
            _cachedPath = null;
        }
    }

    private OnnxGenAIChatClient(string modelPath, string modelName)
    {
        _model = new Model(modelPath);
        _tokenizer = new Tokenizer(_model);
        Metadata = new ChatClientMetadata("OnnxRuntimeGenAI");
    }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _activeInferences);
        return Task.Run(() =>
        {
            try
            {
                if (_disposed) throw new ObjectDisposedException(nameof(OnnxGenAIChatClient));

                var prompt = FormatChatPrompt(chatMessages);
                using var sequences = _tokenizer.Encode(prompt);

                using var genParams = new GeneratorParams(_model);
                genParams.SetSearchOption("max_length", options?.MaxOutputTokens ?? 512);
                genParams.SetSearchOption("temperature", options?.Temperature ?? 0.8);
                genParams.SetSearchOption("top_p", options?.TopP ?? 0.95);

                using var generator = new Generator(_model, genParams);
                generator.AppendTokenSequences(sequences);
                using var stream = _tokenizer.CreateStream();

                var output = new StringBuilder();

                while (!generator.IsDone())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    generator.GenerateNextToken();
                    var token = generator.GetSequence(0)[^1];
                    output.Append(stream.Decode(token));
                }

                var text = CleanOutput(output.ToString());
                return new ChatResponse(new ChatMessage(ChatRole.Assistant, text));
            }
            finally
            {
                Interlocked.Decrement(ref _activeInferences);
            }
        }, cancellationToken);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(chatMessages, options, cancellationToken);
        yield return new ChatResponseUpdate(ChatRole.Assistant, response.Text ?? "");
    }

    public object? GetService(Type serviceType, object? key = null)
        => serviceType.IsAssignableFrom(GetType()) ? this : null;

    private void DisposeInternal()
    {
        if (_disposed) return;
        _disposed = true;
        _tokenizer?.Dispose();
        _model?.Dispose();
    }

    private static string FormatChatPrompt(IEnumerable<ChatMessage> messages)
    {
        var sb = new StringBuilder();
        foreach (var msg in messages)
        {
            var role = msg.Role == ChatRole.System ? "system"
                     : msg.Role == ChatRole.User ? "user"
                     : "assistant";
            sb.Append($"<|{role}|>\n{msg.Text}<|end|>\n");
        }
        sb.Append("<|assistant|>\n");
        return sb.ToString();
    }

    private static string CleanOutput(string text)
    {
        text = text.Trim();
        foreach (var marker in new[] { "<|end|>", "<|endoftext|>", "<|assistant|>" })
        {
            var idx = text.IndexOf(marker, StringComparison.Ordinal);
            if (idx >= 0) text = text[..idx].Trim();
        }
        return text;
    }
}
#endif

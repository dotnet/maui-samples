using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalChatClientWithTools.Services.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace LocalChatClientWithTools.ViewModels;

public partial class ChatViewModel(
    IChatClient chatClient,
    IEnumerable<AIFunction> tools,
    ILogger<ChatViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isProcessing = false;

    public ObservableCollection<ChatMessageViewModel> Messages { get; } = [];

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || IsProcessing)
            return;

        IsProcessing = true;

        try
        {
            Messages.Add(new TextMessageViewModel { IsUser = true, Text = text });
            InputText = string.Empty;

            // Streaming placeholder — always the last message in the collection.
            Messages.Add(new TextMessageViewModel { Text = "⏳" });

            var chatOptions = new ChatOptions
            {
                Tools = WrapToolsWithNotifications(tools)
            };

            var textBuilder = new StringBuilder();

            await foreach (var update in chatClient.GetStreamingResponseAsync(text, chatOptions))
            {
                if (string.IsNullOrEmpty(update.Text))
                    continue;

                textBuilder.Append(update.Text);
                Messages[^1] = new TextMessageViewModel
                {
                    Text = textBuilder.ToString()
                };
            }

            if (textBuilder.Length == 0)
                Messages.RemoveAt(Messages.Count - 1);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during chat completion");
            Messages[^1] = new TextMessageViewModel
            {
                Text = $"❌ Error: {ex.Message}",
                IsError = true
            };
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task SendPrompt(string? prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt) || IsProcessing)
            return;

        InputText = prompt.Trim();

        if (CanSend())
            await Send();
    }

    [RelayCommand]
    private void ClearChat() => Messages.Clear();

    // ── Tool observability ───────────────────────────────────────────

    private IList<AITool> WrapToolsWithNotifications(IEnumerable<AIFunction> tools) =>
        [.. tools.Select(t => new ObservableAIFunction(t, OnToolInvoking, OnToolInvoked))];

    private void OnToolInvoking(string toolName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var insertIndex = Math.Max(0, Messages.Count - 1);
            Messages.Insert(insertIndex, new ToolCallMessageViewModel
            {
                ToolName = toolName,
                Text = $"🔧 Calling {toolName}…"
            });
        });
    }

    private void OnToolInvoked(string toolName, AIFunctionArguments arguments, object? result)
    {
        var rawJson = BuildRawJson(arguments, result);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            for (int i = Messages.Count - 1; i >= 0; i--)
            {
                if (Messages[i] is ToolCallMessageViewModel tc && tc.ToolName == toolName
                    && !tc.IsCompleted)
                {
                    tc.Text = $"✅ {toolName} completed";
                    tc.RawJson = rawJson;
                    tc.IsCompleted = true;
                    break;
                }
            }
        });
    }

    private static string? BuildRawJson(AIFunctionArguments arguments, object? result)
    {
        var sb = new StringBuilder();
        var indented = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        try
        {
            if (arguments.Count > 0)
            {
                var argsDict = new Dictionary<string, object?>();
                foreach (var kv in arguments) argsDict[kv.Key] = kv.Value;
                sb.AppendLine("── Arguments ──");
                sb.AppendLine(JsonSerializer.Serialize(argsDict, indented));
            }
        }
        catch { }

        try
        {
            if (result is string jsonString)
            {
                var parsed = JsonDocument.Parse(jsonString);
                if (sb.Length > 0) sb.AppendLine();
                sb.AppendLine("── Result ──");
                sb.Append(JsonSerializer.Serialize(parsed.RootElement, indented));
            }
            else if (result is not null)
            {
                var json = JsonSerializer.Serialize(result, result.GetType(), ToolJsonContext.Default.Options);
                var parsed = JsonDocument.Parse(json);
                if (sb.Length > 0) sb.AppendLine();
                sb.AppendLine("── Result ──");
                sb.Append(JsonSerializer.Serialize(parsed.RootElement, indented));
            }
        }
        catch
        {
            if (result is not null)
            {
                if (sb.Length > 0) sb.AppendLine();
                sb.AppendLine("── Result ──");
                sb.Append(result);
            }
        }

        return sb.Length > 0 ? sb.ToString() : null;
    }

    private bool CanSend() =>
        !string.IsNullOrWhiteSpace(InputText) && !IsProcessing;

    // ── Observable tool wrapper ──────────────────────────────────────

    private sealed class ObservableAIFunction(
        AIFunction inner,
        Action<string> onInvoking,
        Action<string, AIFunctionArguments, object?> onInvoked)
        : AIFunction
    {
        public override string Name => inner.Name;
        public override string Description => inner.Description;
        public override JsonElement JsonSchema => inner.JsonSchema;
        public override JsonElement? ReturnJsonSchema => inner.ReturnJsonSchema;
        public override JsonSerializerOptions JsonSerializerOptions => inner.JsonSerializerOptions;
        public override IReadOnlyDictionary<string, object?> AdditionalProperties => inner.AdditionalProperties;

        protected override async ValueTask<object?> InvokeCoreAsync(
            AIFunctionArguments arguments,
            CancellationToken cancellationToken)
        {
            onInvoking(inner.Name);
            object? result = null;
            try
            {
                result = await inner.InvokeAsync(arguments, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                result = $"{{\"error\": \"{ex.GetType().Name}: {ex.Message.Replace("\"", "\\\"").Replace("\n", " ")}\"}}";
                return result;
            }
            finally
            {
                onInvoked(inner.Name, arguments, result);
            }
        }
    }
}
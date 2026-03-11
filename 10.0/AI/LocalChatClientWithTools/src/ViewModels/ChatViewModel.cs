using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalChatClientWithTools.Models;
using LocalChatClientWithTools.Services.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using UiChatMessage = LocalChatClientWithTools.Models.ChatMessage;

namespace LocalChatClientWithTools.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isProcessing = false;

    public ObservableCollection<UiChatMessage> Messages { get; } = new();

    private readonly IChatClient _chatClient;
    private readonly ILogger<ChatViewModel> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ChatViewModel(IChatClient chatClient, ILogger<ChatViewModel> logger, IServiceProvider serviceProvider)
    {
        _chatClient = chatClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || IsProcessing) return;

        IsProcessing = true;

        try
        {
            Messages.Add(new UiChatMessage
            {
                From = Sender.User,
                Text = text,
                Type = MessageType.Text
            });
            InputText = string.Empty;

            // Streaming placeholder — always the last message in the collection.
            // Tool-call indicators are inserted *before* it so the index stays valid.
            Messages.Add(new UiChatMessage
            {
                From = Sender.Assistant,
                Text = "⏳",
                Type = MessageType.Text
            });

            var chatOptions = new ChatOptions
            {
                Tools = WrapToolsWithNotifications(await GetAvailableToolsAsync())
            };

            var textBuilder = new StringBuilder();

            await foreach (var update in _chatClient.GetStreamingResponseAsync(text, chatOptions))
            {
                if (string.IsNullOrEmpty(update.Text))
                    continue;

                textBuilder.Append(update.Text);
                Messages[^1] = new UiChatMessage
                {
                    From = Sender.Assistant,
                    Text = textBuilder.ToString(),
                    Type = MessageType.Text
                };
            }

            if (textBuilder.Length == 0)
            {
                Messages[^1] = new UiChatMessage
                {
                    From = Sender.Assistant,
                    Text = "(no response)",
                    Type = MessageType.Text
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chat completion");
            Messages.Add(new UiChatMessage
            {
                From = Sender.Assistant,
                Text = $"❌ Error: {ex.Message}",
                IsError = true
            });
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
        {
            await Send();
        }
    }

    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
    }

    // ── Tool observability ───────────────────────────────────────────

    private IList<AITool> WrapToolsWithNotifications(IList<AITool> tools)
    {
        return tools.Select(t => t is AIFunction fn
            ? (AITool)new ObservableAIFunction(fn, OnToolInvoking, OnToolInvoked)
            : t).ToList();
    }

    private void OnToolInvoking(string toolName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Insert the tool-call indicator *before* the streaming placeholder
            // so the placeholder remains the last item in the collection.
            var insertIndex = Math.Max(0, Messages.Count - 1);
            Messages.Insert(insertIndex, new UiChatMessage
            {
                From = Sender.Assistant,
                Text = $"🔧 Calling {FormatToolName(toolName)}…",
                Type = MessageType.ToolCall,
                ToolName = toolName
            });
        });
    }

    private void OnToolInvoked(string toolName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            for (int i = Messages.Count - 1; i >= 0; i--)
            {
                if (Messages[i].IsToolCall && Messages[i].ToolName == toolName
                    && Messages[i].Text.StartsWith("🔧"))
                {
                    Messages[i] = new UiChatMessage
                    {
                        From = Sender.Assistant,
                        Text = $"✅ {FormatToolName(toolName)} completed",
                        Type = MessageType.ToolCall,
                        ToolName = toolName
                    };
                    break;
                }
            }
        });
    }

    private static string FormatToolName(string name) => name switch
    {
        "get_weather" => "Weather",
        "calculate" => "Calculator",
        "list_files" => "File Operations",
        "get_system_info" => "System Info",
        "set_timer" => "Timer",
        _ => name
    };

    // ── Helpers ──────────────────────────────────────────────────────

    private Task<IList<AITool>> GetAvailableToolsAsync()
    {
        var tools = new List<AITool>();

        try
        {
            tools.Add(_serviceProvider.GetRequiredService<CalculatorTool>());
            tools.Add(_serviceProvider.GetRequiredService<WeatherTool>());
            tools.Add(_serviceProvider.GetRequiredService<FileOperationsTool>());
            tools.Add(_serviceProvider.GetRequiredService<SystemInfoTool>());
            tools.Add(_serviceProvider.GetRequiredService<TimerTool>());

            _logger.LogDebug("Retrieved {ToolCount} tools for chat completion", tools.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving tools for chat completion");
        }

        return Task.FromResult<IList<AITool>>(tools);
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText) && !IsProcessing;

    // ── Observable tool wrapper ──────────────────────────────────────

    /// <summary>
    /// Wraps an <see cref="AIFunction"/> to fire callbacks when the tool is
    /// invoked, giving the UI a chance to show real-time tool-call status.
    /// </summary>
    private sealed class ObservableAIFunction : AIFunction
    {
        private readonly AIFunction _inner;
        private readonly Action<string> _onInvoking;
        private readonly Action<string> _onInvoked;

        public ObservableAIFunction(AIFunction inner, Action<string> onInvoking, Action<string> onInvoked)
        {
            _inner = inner;
            _onInvoking = onInvoking;
            _onInvoked = onInvoked;
        }

        public override string Name => _inner.Name;
        public override string Description => _inner.Description;
        public override JsonElement JsonSchema => _inner.JsonSchema;

        protected override async ValueTask<object?> InvokeCoreAsync(
            AIFunctionArguments arguments,
            CancellationToken cancellationToken)
        {
            _onInvoking(_inner.Name);
            try
            {
                return await _inner.InvokeAsync(arguments, cancellationToken);
            }
            finally
            {
                _onInvoked(_inner.Name);
            }
        }
    }
}
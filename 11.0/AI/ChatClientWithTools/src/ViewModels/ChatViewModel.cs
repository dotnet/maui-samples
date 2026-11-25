using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatClientWithTools.Models;
using ChatClientWithTools.Services.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using UiChatMessage = ChatClientWithTools.Models.ChatMessage;

namespace ChatClientWithTools.ViewModels;

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
            // Add the user's message
            var userMessage = new UiChatMessage
            {
                From = Sender.User,
                Text = text,
                Type = MessageType.Text
            };
            Messages.Add(userMessage);


            // Clear input
            InputText = string.Empty;

            // Add a placeholder assistant message 
            var placeholder = new UiChatMessage
            {
                From = Sender.Assistant,
                Text = "🤔 Thinking...",
                Type = MessageType.Text
            };
            Messages.Add(placeholder);
            var placeholderIndex = Messages.IndexOf(placeholder);

            // Call AI and update the placeholder with assistant response
            try
            {
                // Include tools in the chat options
                var chatOptions = new ChatOptions
                {
                    Tools = await GetAvailableToolsAsync()
                };

                var response = await _chatClient.GetResponseAsync(text, chatOptions);
                var reply = response?.Messages?.LastOrDefault()?.Text ?? "(no response)";

                if (placeholderIndex >= 0 && placeholderIndex < Messages.Count)
                {
                    Messages[placeholderIndex] = new UiChatMessage { From = Sender.Assistant, Text = reply };
                }
                else
                {
                    Messages.Add(new UiChatMessage { From = Sender.Assistant, Text = reply });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during chat completion");
                var errorMessage = $"❌ Error: {ex.Message}";

                if (placeholderIndex >= 0 && placeholderIndex < Messages.Count)
                {
                    Messages[placeholderIndex] = new UiChatMessage
                    {
                        From = Sender.Assistant,
                        Text = errorMessage,
                        IsError = true
                    };
                }
                else
                {
                    Messages.Add(new UiChatMessage
                    {
                        From = Sender.Assistant,
                        Text = errorMessage,
                        IsError = true
                    });
                }
            }
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

    private Task<IList<AITool>> GetAvailableToolsAsync()
    {
        var tools = new List<AITool>();

        try
        {
            // Get all registered tools from DI
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
}
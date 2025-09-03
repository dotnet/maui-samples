using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatMobile.Client.Models;
using ChatMobile.Client.Services;
using Microsoft.Extensions.Logging;

namespace ChatMobile.Client.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isProcessing = false;

    [ObservableProperty]
    private bool _isConfigured = false;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    private readonly IChatService _chatService;
    private readonly ILogger<ChatViewModel> _logger;

    public ChatViewModel(IChatService chatService, ILogger<ChatViewModel> logger)
    {
        _chatService = chatService;
        _logger = logger;
        
        // Check if configured on startup
        _ = Task.Run(async () =>
        {
            IsConfigured = await _chatService.IsConfiguredAsync();
            
            if (!IsConfigured)
            {
                Messages.Add(new ChatMessage
                {
                    From = Sender.Assistant,
                    Text = "⚠️ Welcome! Please go to the Setup tab to configure your API credentials before chatting.",
                    IsError = false
                });
            }
        });
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || IsProcessing) return;

        IsProcessing = true;

        try
        {
            // Check if configured
            IsConfigured = await _chatService.IsConfiguredAsync();
            if (!IsConfigured)
            {
                Messages.Add(new ChatMessage
                {
                    From = Sender.Assistant,
                    Text = "⚠️ Please set up your API credentials in the Setup tab first.",
                    IsError = true
                });
                return;
            }

            // Add the user's message
            var userMessage = new ChatMessage
            {
                From = Sender.User,
                Text = text,
                Type = MessageType.Text
            };
            Messages.Add(userMessage);

            // Clear input
            InputText = string.Empty;

            // Add a placeholder assistant message 
            var placeholder = new ChatMessage
            {
                From = Sender.Assistant,
                Text = "🤔 Thinking...",
                Type = MessageType.Text
            };
            Messages.Add(placeholder);
            var placeholderIndex = Messages.IndexOf(placeholder);

            // Call ChatService and update the placeholder
            try
            {
                var response = await _chatService.SendMessageAsync(text, Messages.Where(m => m != placeholder).ToList());
                
                // Replace placeholder with actual response
                UpdatePlaceholder(placeholderIndex, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during chat completion");
                
                UpdatePlaceholder(placeholderIndex, new ChatMessage
                {
                    From = Sender.Assistant,
                    Text = $"❌ Error: {ex.Message}",
                    IsError = true
                });
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

    [RelayCommand]
    private async Task RefreshConfiguration()
    {
        // Refresh the chat service to pick up new credentials
        _chatService.RefreshClient();
        IsConfigured = await _chatService.IsConfiguredAsync();
        
        if (IsConfigured)
        {
            // Remove any configuration warning messages
            var warningMessages = Messages.Where(m => m.Text.Contains("⚠️") && m.Text.Contains("Setup")).ToList();
            foreach (var warning in warningMessages)
            {
                Messages.Remove(warning);
            }
            
            Messages.Add(new ChatMessage
            {
                From = Sender.Assistant,
                Text = "✅ Configuration updated! You can now start chatting.",
                IsError = false
            });
        }
    }

    private void UpdatePlaceholder(int placeholderIndex, ChatMessage newMessage)
    {
        if (placeholderIndex >= 0 && placeholderIndex < Messages.Count)
        {
            Messages[placeholderIndex] = newMessage;
        }
        else
        {
            Messages.Add(newMessage);
        }
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText) && !IsProcessing;
}
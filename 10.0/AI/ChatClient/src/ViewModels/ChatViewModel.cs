using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleChatClient.Models;
using Microsoft.Extensions.AI;
using UiChatMessage = SimpleChatClient.Models.ChatMessage;

namespace SimpleChatClient.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    public ObservableCollection<UiChatMessage> Messages { get; } = new();

    private readonly IChatClient _chatClient;

    public ChatViewModel(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text)) return;
        // Add the user's message
        Messages.Add(new UiChatMessage { From = Sender.User, Text = text });

        // Clear input
        InputText = string.Empty;

        // Add a placeholder assistant message 
        var placeholder = new UiChatMessage { From = Sender.Assistant, Text = "..." };
        Messages.Add(placeholder);
        var placeholderIndex = Messages.IndexOf(placeholder);

        // Call AI and update the placeholder with assistant response
        try
        {
            var response = await _chatClient.GetResponseAsync(text);
            var reply = string.IsNullOrWhiteSpace(response.Text) ? "(no response)" : response.Text;
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
            var err = $"Error: {ex.Message}";
            if (placeholderIndex >= 0 && placeholderIndex < Messages.Count)
            {
                Messages[placeholderIndex] = new UiChatMessage { From = Sender.Assistant, Text = err };
            }
            else
            {
                Messages.Add(new UiChatMessage { From = Sender.Assistant, Text = err });
            }
        }
    }

    // Triggers sending a predefined prompt from UI buttons
    [RelayCommand]
    private async Task SendPrompt(string? prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            return;

        InputText = prompt.Trim();
        if (CanSend())
        {
            await Send();
        }
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText);
}

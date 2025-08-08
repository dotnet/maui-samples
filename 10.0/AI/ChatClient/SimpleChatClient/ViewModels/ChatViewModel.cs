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
        // Seed with a greeting from the assistant
        Messages.Add(new UiChatMessage
        {
            From = Sender.Assistant,
            Text = "Hi! Ask me anything."
        });
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

        // Call AI and add assistant response
        try
        {
            var response = await _chatClient.GetResponseAsync(text);
            var reply = string.IsNullOrWhiteSpace(response.Text) ? "(no response)" : response.Text;
            Messages.Add(new UiChatMessage { From = Sender.Assistant, Text = reply });
        }
        catch (Exception ex)
        {
            Messages.Add(new UiChatMessage { From = Sender.Assistant, Text = $"Error: {ex.Message}" });
        }
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText);
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleChatClient.Models;

namespace SimpleChatClient.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public ChatViewModel()
    {
        // Seed with a greeting from the assistant
        Messages.Add(new ChatMessage
        {
            From = Sender.Assistant,
            Text = "Hi! Ask me anything."
        });
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private void Send()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        // Add the user's message
        Messages.Add(new ChatMessage { From = Sender.User, Text = text });

        // Clear input
        InputText = string.Empty;

        // Placeholder assistant response (will be replaced by OpenAI in a later step)
        Messages.Add(new ChatMessage { From = Sender.Assistant, Text = "(Thinking…)" });
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText);
}

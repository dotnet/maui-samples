using CommunityToolkit.Mvvm.ComponentModel;

namespace LocalChatClientWithTools.ViewModels;

public abstract partial class ChatMessageViewModel : ObservableObject
{
    public bool IsUser { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}

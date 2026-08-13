namespace LocalChatClientWithTools.ViewModels;

public class TextMessageViewModel : ChatMessageViewModel
{
    public required string Text { get; set; }

    public bool IsError { get; init; }
}

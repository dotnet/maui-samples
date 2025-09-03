using ChatVoice.Client.Models;

namespace ChatVoice.Client.Services;

public interface IChatService
{
    Task<ChatMessage> SendMessageAsync(string message, IList<ChatMessage> history, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync();
    void RefreshClient();
}

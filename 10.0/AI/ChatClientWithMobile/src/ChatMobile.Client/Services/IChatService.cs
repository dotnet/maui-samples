using Microsoft.Extensions.AI;
using ChatMobile.Client.Models;

namespace ChatMobile.Client.Services;

public interface IChatService
{
    Task<Models.ChatMessage> SendMessageAsync(string message, IList<Models.ChatMessage> conversationHistory, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync();
    void RefreshClient();
}
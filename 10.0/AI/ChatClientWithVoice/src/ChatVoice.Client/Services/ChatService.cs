using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using ChatVoice.Client.Models;
using ChatVoice.Client.Tools;
using Microsoft.Extensions.Logging;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ModelChatMessage = ChatVoice.Client.Models.ChatMessage;

namespace ChatVoice.Client.Services;

public class ChatService(IApiService api, ISecureCredentialService secure, ILogger<ChatService> logger) : IChatService
{
    private IChatClient? _client;
    private readonly IApiService _api = api;
    private readonly ISecureCredentialService _secure = secure;
    private readonly ILogger<ChatService> _logger = logger;

    public async Task<ModelChatMessage> SendMessageAsync(string message, IList<ModelChatMessage> conversationHistory, CancellationToken cancellationToken = default)
    {
        var client = await GetClientAsync(cancellationToken);
        if (client is null)
        {
            return new ModelChatMessage
            {
                Text = "❌ No credentials configured. Please use the Setup tab.",
                From = Sender.Assistant,
                IsError = true
            };
        }

        // Build chat history (user/assistant plain text only)
        var chat = new List<AiChatMessage>();
        foreach (var m in conversationHistory.Where(m => m.Type == MessageType.Text))
        {
            var role = m.From == Sender.User ? ChatRole.User : ChatRole.Assistant;
            chat.Add(new AiChatMessage(role, m.Text));
        }
        chat.Add(new AiChatMessage(ChatRole.User, message));

        // Build tool list per request
        var tools = await BuildToolsAsync(cancellationToken);
        var options = new ChatOptions { Tools = tools };

        var response = await client.GetResponseAsync(chat, options, cancellationToken);

        return new ModelChatMessage
        {
            Text = response.Text ?? "(no response)",
            From = Sender.Assistant
        };
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return await _secure.HasCredentialsAsync();
    }

    public void RefreshClient()
    {
        _client?.Dispose();
        _client = null;
    }

    private async Task<IChatClient?> GetClientAsync(CancellationToken ct)
    {
        if (_client != null) return _client;
        // Try secure storage first
        var endpoint = await _secure.GetAzureOpenAIEndpointAsync();
        var apiKey = await _secure.GetAzureOpenAIApiKeyAsync();
        var model = await _secure.GetAzureOpenAIModelAsync();
        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
        {
            var creds = await _api.GetCredentialsAsync(ct);
            if (creds is null) return null;
            endpoint = creds.AzureOpenAI.Endpoint;
            apiKey = creds.AzureOpenAI.ApiKey;
            model = creds.AzureOpenAI.Model;
            if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey))
            {
                await _secure.StoreCredentialsAsync(endpoint!, apiKey!, model ?? "gpt-4o-mini", creds.WeatherApiKey);
            }
        }
        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey)) return null;

        var aoai = new AzureOpenAIClient(new Uri(endpoint!), new System.ClientModel.ApiKeyCredential(apiKey!));
        var azureChatClient = aoai.GetChatClient(model ?? "gpt-4o-mini");
        _client = new ChatClientBuilder(azureChatClient.AsIChatClient())
            .UseFunctionInvocation()
            .Build();
        return _client;
    }

    private async Task<IList<AITool>> BuildToolsAsync(CancellationToken cancellationToken)
    {
        var tools = new List<AITool>();

        // Calculator (local only)
        tools.Add(new CalculatorTool());

        // Weather (uses OpenWeatherMap through geocoding + weather by lat/lon)
        var weatherApiKey = await _secure.GetWeatherApiKeyAsync();
        var httpClient = new HttpClient();
        tools.Add(new WeatherTool(httpClient, weatherApiKey));

        // Files, System info, Timer
        tools.Add(new FileOperationsTool());
        tools.Add(new SystemInfoTool());
        tools.Add(new TimerTool());

        return tools;
    }
}

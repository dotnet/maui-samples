using Microsoft.Extensions.AI;
using ChatMobile.Client.Models;
using ChatMobile.Client.Tools;
using Azure.AI.OpenAI;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ChatMobile.Client.Services;

public class ChatService : IChatService
{
    private readonly ISecureCredentialService _credentialService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ChatService> _logger;
    private IChatClient? _chatClient;

    public ChatService(ISecureCredentialService credentialService, IHttpClientFactory httpClientFactory, ILogger<ChatService> logger)
    {
        _credentialService = credentialService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return await _credentialService.HasCredentialsAsync();
    }

    public void RefreshClient()
    {
        // Clear the cached client so it gets recreated with new credentials
        _chatClient?.Dispose();
        _chatClient = null;
    }

    public async Task<Models.ChatMessage> SendMessageAsync(string message, IList<Models.ChatMessage> conversationHistory, CancellationToken cancellationToken = default)
    {
        var hasCredentials = await _credentialService.HasCredentialsAsync();
        if (!hasCredentials)
        {
            return new Models.ChatMessage
            {
                Text = "❌ No credentials configured. Please set up your API keys first.",
                From = Sender.Assistant,
                IsError = true
            };
        }

        try
        {
            // Get or create the chat client with current credentials
            var chatClient = await GetOrCreateChatClientAsync();
            if (chatClient == null)
            {
                return new Models.ChatMessage
                {
                    Text = "❌ Failed to initialize AI client. Please check your credentials.",
                    From = Sender.Assistant,
                    IsError = true
                };
            }

            var chatHistory = BuildChatHistory(conversationHistory, message);

            // Build tool list for this request
            var chatOptions = new ChatOptions
            {
                Tools = await BuildToolsAsync(cancellationToken)
            };

            var response = await chatClient.GetResponseAsync(chatHistory, chatOptions, cancellationToken);

            return new Models.ChatMessage
            {
                Text = response.Text ?? "(no response)",
                From = Sender.Assistant
            };
        }
        catch (Exception ex)
        {
            return new Models.ChatMessage
            {
                Text = $"❌ Error: {ex.Message}",
                From = Sender.Assistant,
                IsError = true
            };
        }
    }

    private List<Microsoft.Extensions.AI.ChatMessage> BuildChatHistory(IList<Models.ChatMessage> conversationHistory, string newMessage)
    {
        var chatHistory = new List<Microsoft.Extensions.AI.ChatMessage>();

        // Add conversation history (excluding tool-related messages for simplicity in this initial implementation)
        foreach (var msg in conversationHistory.Where(m => m.Type == MessageType.Text))
        {
            var role = msg.From == Sender.User ? ChatRole.User : ChatRole.Assistant;
            chatHistory.Add(new Microsoft.Extensions.AI.ChatMessage(role, msg.Text));
        }

        // Add new message
        chatHistory.Add(new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, newMessage));

        return chatHistory;
    }

    private async Task<IChatClient?> GetOrCreateChatClientAsync()
    {
        if (_chatClient != null)
        {
            return _chatClient;
        }

        try
        {
            _logger.LogDebug("Attempting to create Azure OpenAI chat client...");

            var endpoint = await _credentialService.GetAzureOpenAIEndpointAsync();
            var apiKey = await _credentialService.GetAzureOpenAIApiKeyAsync();
            var model = await _credentialService.GetAzureOpenAIModelAsync() ?? "gpt-5-mini";
            _logger.LogDebug("Retrieved credentials - Endpoint: {Endpoint}, Model: {Model}, HasApiKey: {HasApiKey}",
                endpoint, model, !string.IsNullOrWhiteSpace(apiKey));

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("Missing Azure OpenAI credentials - Endpoint: {Endpoint}, HasApiKey: {HasApiKey}",
                    endpoint, !string.IsNullOrWhiteSpace(apiKey));
                return null;
            }

            _logger.LogDebug("Creating Azure OpenAI client with endpoint: {Endpoint}", endpoint);
            var aoai = new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey));

            _logger.LogDebug("Getting chat client for model: {Model}", model);
            var azureChatClient = aoai.GetChatClient(model);

            _logger.LogDebug("Creating IChatClient using AsIChatClient extension...");
            _chatClient = new ChatClientBuilder(azureChatClient.AsIChatClient())
                .UseFunctionInvocation()
                .Build();

            _logger.LogDebug("Azure OpenAI chat client created successfully");
            return _chatClient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Azure OpenAI chat client: {ErrorMessage}", ex.Message);
            return null;
        }
    }

    private async Task<IList<AITool>> BuildToolsAsync(CancellationToken cancellationToken)
    {
        var tools = new List<AITool>();

        // Calculator (pure local)
        tools.Add(new CalculatorTool());

        // Weather (needs optional API key)
        var weatherApiKey = await _credentialService.GetWeatherApiKeyAsync();
        var httpClient = _httpClientFactory.CreateClient();
        tools.Add(new WeatherTool(httpClient, weatherApiKey));

        // Files, System info, Timer
        tools.Add(new FileOperationsTool());
        tools.Add(new SystemInfoTool());
        tools.Add(new TimerTool());

        return tools;
    }
}
using System.Text;
using System.Text.Json;
using ChatMobile.Client.Models;
using Microsoft.Extensions.Logging;

namespace ChatMobile.Client.Services;

public interface IApiService
{
    Task<ChatResponse> SendMessageAsync(string message, CancellationToken cancellationToken = default);
    Task<ChatResponse> SendToolResultAsync(string requestId, string toolName, object result, string? originalMessage = null, CancellationToken cancellationToken = default);
    Task<CredentialResponse?> GetCredentialsAsync(CancellationToken cancellationToken = default);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<ChatResponse> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ChatRequest
            {
                Message = message
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending message to API: {Message}", message);
            
            var response = await _httpClient.PostAsync("api/chat/send", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseJson, _jsonOptions);
                
                if (chatResponse != null)
                {
                    _logger.LogInformation("Received response from API with {ClientToolCount} client tool requests and {ServerToolCount} server tool results",
                        chatResponse.ClientToolRequests.Count, chatResponse.ServerToolResults.Count);
                    return chatResponse;
                }
            }
            
            _logger.LogWarning("API request failed with status: {StatusCode}", response.StatusCode);
            
            return new ChatResponse
            {
                Message = "Failed to get response from server",
                IsError = true,
                ErrorMessage = $"HTTP {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to API");
            return new ChatResponse
            {
                Message = "Failed to connect to server",
                IsError = true,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<ChatResponse> SendToolResultAsync(string requestId, string toolName, object result, string? originalMessage = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ToolExecutionRequest
            {
                RequestId = requestId,
                ToolName = toolName,
                Result = result,
                OriginalMessage = originalMessage
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending tool result to API: {ToolName}", toolName);
            
            var response = await _httpClient.PostAsync("api/chat/execute-tool", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseJson, _jsonOptions);
                
                if (chatResponse != null)
                {
                    return chatResponse;
                }
            }
            
            _logger.LogWarning("Tool result API request failed with status: {StatusCode}", response.StatusCode);
            
            return new ChatResponse
            {
                Message = "Failed to process tool result on server",
                IsError = true,
                ErrorMessage = $"HTTP {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending tool result to API");
            return new ChatResponse
            {
                Message = "Failed to send tool result to server",
                IsError = true,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<CredentialResponse?> GetCredentialsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching credentials from API");
            
            var response = await _httpClient.GetAsync("api/credentials", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var credentials = JsonSerializer.Deserialize<CredentialResponse>(responseJson, _jsonOptions);
                
                _logger.LogInformation("Successfully fetched credentials from API");
                return credentials;
            }
            
            _logger.LogWarning("Credentials API request failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching credentials from API");
            return null;
        }
    }
}
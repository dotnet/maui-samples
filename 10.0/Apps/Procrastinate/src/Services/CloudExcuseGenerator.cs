using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace procrastinate.Services;

public class CloudExcuseGenerator : IExcuseGenerator
{
    public string Name => "Cloud AI (Groq)";
    public bool IsAvailable => !string.IsNullOrEmpty(ApiKey);
    
    private static string ApiKey => Preferences.Get("GroqApiKey", "");
    private static string ApiEndpoint => Preferences.Get("GroqApiEndpoint", "https://api.groq.com/openai/v1/chat/completions");
    private static string Model => Preferences.Get("GroqModel", "llama-3.3-70b-versatile");
    private const int MaxRetries = 3;
    
    private static readonly Lazy<HttpClient> _httpClient = new(() => 
        new HttpClient { Timeout = TimeSpan.FromSeconds(30) });

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();
        
        if (!IsAvailable)
            return new ExcuseResult("Cloud AI requires an API key. Configure it in Settings.", Name, stopwatch.Elapsed);

        try
        {
            var languageName = language switch
            {
                "fr" => "French",
                "es" => "Spanish",
                "pt" => "Portuguese",
                "nl" => "Dutch",
                "cs" => "Czech",
                "uk" => "Ukrainian",
                _ => "English"
            };

            var prompt = $"Generate a single funny, creative excuse for not doing work or being productive. The excuse should be absurd but delivered with a straight face. Write it in {languageName}. Reply with ONLY the excuse text, no quotes or explanation.";

            var request = new GroqRequest
            {
                Model = Model,
                Messages = new[]
                {
                    new GroqMessage { Role = "user", Content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(request);
            
            Exception? lastException = null;
            for (int retry = 0; retry < MaxRetries; retry++)
            {
                try
                {
                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiEndpoint)
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    };
                    httpRequest.Headers.Add("Authorization", $"Bearer {ApiKey}");
                    
                    var response = await _httpClient.Value.SendAsync(httpRequest);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        stopwatch.Stop();
                        return new ExcuseResult($"API error ({response.StatusCode}): {errorBody}", Name, stopwatch.Elapsed, Model: Model);
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<GroqResponse>(responseBody);
                    stopwatch.Stop();
                    
                    var excuse = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim() ?? "The AI is also procrastinating...";
                    var tokenCount = result?.Usage?.TotalTokens;
                    
                    return new ExcuseResult(excuse, Name, stopwatch.Elapsed, tokenCount, Model);
                }
                catch (Exception ex) when (retry < MaxRetries - 1)
                {
                    lastException = ex;
                    await Task.Delay(1000 * (retry + 1)); // Exponential backoff
                }
            }
            
            stopwatch.Stop();
            return new ExcuseResult($"Connection failed: {lastException?.Message ?? "Unknown error"}", Name, stopwatch.Elapsed, Model: Model);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ExcuseResult($"Cloud excuse failed: {ex.Message}", Name, stopwatch.Elapsed, Model: Model);
        }
    }

    private class GroqRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "";
        [JsonPropertyName("messages")]
        public GroqMessage[] Messages { get; set; } = Array.Empty<GroqMessage>();
    }

    private class GroqMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "";
        [JsonPropertyName("content")]
        public string Content { get; set; } = "";
    }

    private class GroqResponse
    {
        [JsonPropertyName("choices")]
        public GroqChoice[]? Choices { get; set; }
        [JsonPropertyName("usage")]
        public GroqUsage? Usage { get; set; }
    }

    private class GroqChoice
    {
        [JsonPropertyName("message")]
        public GroqMessage? Message { get; set; }
    }
    
    private class GroqUsage
    {
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}

namespace ChatVoice.Api.Models;

public class CredentialResponse
{
    public AzureOpenAICredentials AzureOpenAI { get; set; } = new();
    public string WeatherApiKey { get; set; } = string.Empty;
}

public class AzureOpenAICredentials
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
}

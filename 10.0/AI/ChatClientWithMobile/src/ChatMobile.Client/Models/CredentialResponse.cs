namespace ChatMobile.Client.Models;

public class CredentialResponse
{
    public required AzureOpenAICredentials AzureOpenAI { get; set; }
    public required string WeatherApiKey { get; set; }
}

public class AzureOpenAICredentials
{
    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
    public required string Model { get; set; }
}
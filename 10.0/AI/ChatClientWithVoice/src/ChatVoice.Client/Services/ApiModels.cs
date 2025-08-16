namespace ChatVoice.Client.Services;

public record AzureOpenAICredentials(string Endpoint, string ApiKey, string Model);
public record CredentialResponse(AzureOpenAICredentials AzureOpenAI, string WeatherApiKey);

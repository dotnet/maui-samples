using ChatMobile.Api.Models;

namespace ChatMobile.Api.Services;

public class CredentialService : ICredentialService
{
    private readonly IConfiguration _configuration;

    public CredentialService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CredentialResponse GetCredentials()
    {
        var endpoint = _configuration["AzureOpenAI:Endpoint"];
        var apiKey = _configuration["AzureOpenAI:ApiKey"];
        var model = _configuration["AzureOpenAI:Model"];
        var weatherApiKey = _configuration["WeatherApiKey"];

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(weatherApiKey))
        {
            throw new InvalidOperationException("Missing required configuration values");
        }

        return new CredentialResponse
        {
            AzureOpenAI = new AzureOpenAICredentials
            {
                Endpoint = endpoint,
                ApiKey = apiKey,
                Model = model ?? "gpt-4o-mini"
            },
            WeatherApiKey = weatherApiKey
        };
    }
}
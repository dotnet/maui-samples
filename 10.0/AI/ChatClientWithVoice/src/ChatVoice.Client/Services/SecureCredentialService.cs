namespace ChatVoice.Client.Services;

public class SecureCredentialService : ISecureCredentialService
{
    private const string AZURE_ENDPOINT_KEY = "azure_openai_endpoint";
    private const string AZURE_API_KEY = "azure_openai_api_key";
    private const string AZURE_MODEL_KEY = "azure_openai_model";
    private const string WEATHER_API_KEY = "weather_api_key";

    public async Task<bool> HasCredentialsAsync()
    {
        try
        {
            var endpoint = await SecureStorage.GetAsync(AZURE_ENDPOINT_KEY);
            var apiKey = await SecureStorage.GetAsync(AZURE_API_KEY);
            var weatherKey = await SecureStorage.GetAsync(WEATHER_API_KEY);
            return !string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(weatherKey);
        }
        catch { return false; }
    }

    public Task<string?> GetAzureOpenAIEndpointAsync() => SecureStorage.GetAsync(AZURE_ENDPOINT_KEY);
    public Task<string?> GetAzureOpenAIApiKeyAsync() => SecureStorage.GetAsync(AZURE_API_KEY);
    public Task<string?> GetAzureOpenAIModelAsync() => SecureStorage.GetAsync(AZURE_MODEL_KEY);
    public Task<string?> GetWeatherApiKeyAsync() => SecureStorage.GetAsync(WEATHER_API_KEY);

    public async Task StoreCredentialsAsync(string azureEndpoint, string azureApiKey, string azureModel, string weatherApiKey)
    {
        await SecureStorage.SetAsync(AZURE_ENDPOINT_KEY, azureEndpoint);
        await SecureStorage.SetAsync(AZURE_API_KEY, azureApiKey);
        await SecureStorage.SetAsync(AZURE_MODEL_KEY, azureModel);
        await SecureStorage.SetAsync(WEATHER_API_KEY, weatherApiKey);
    }

    public Task ClearCredentialsAsync()
    {
        SecureStorage.Remove(AZURE_ENDPOINT_KEY);
        SecureStorage.Remove(AZURE_API_KEY);
        SecureStorage.Remove(AZURE_MODEL_KEY);
        SecureStorage.Remove(WEATHER_API_KEY);
        return Task.CompletedTask;
    }
}

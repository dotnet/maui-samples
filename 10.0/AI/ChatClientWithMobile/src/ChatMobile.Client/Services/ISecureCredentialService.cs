namespace ChatMobile.Client.Services;

public interface ISecureCredentialService
{
    Task<bool> HasCredentialsAsync();
    Task<string?> GetAzureOpenAIEndpointAsync();
    Task<string?> GetAzureOpenAIApiKeyAsync();
    Task<string?> GetAzureOpenAIModelAsync();
    Task<string?> GetWeatherApiKeyAsync();
    Task StoreCredentialsAsync(string azureEndpoint, string azureApiKey, string azureModel, string weatherApiKey);
    Task ClearCredentialsAsync();
}
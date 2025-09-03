namespace ChatMobile.Client.Services;

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
            
            return !string.IsNullOrWhiteSpace(endpoint) && 
                   !string.IsNullOrWhiteSpace(apiKey) && 
                   !string.IsNullOrWhiteSpace(weatherKey);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string?> GetAzureOpenAIEndpointAsync()
    {
        try
        {
            var result = await SecureStorage.GetAsync(AZURE_ENDPOINT_KEY);
            System.Diagnostics.Debug.WriteLine($"SecureCredentialService: GetAzureOpenAIEndpointAsync returned: {result}");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SecureCredentialService: GetAzureOpenAIEndpointAsync failed: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetAzureOpenAIApiKeyAsync()
    {
        try
        {
            var result = await SecureStorage.GetAsync(AZURE_API_KEY);
            System.Diagnostics.Debug.WriteLine($"SecureCredentialService: GetAzureOpenAIApiKeyAsync returned: {(string.IsNullOrWhiteSpace(result) ? "null/empty" : "***key***")}");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SecureCredentialService: GetAzureOpenAIApiKeyAsync failed: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetAzureOpenAIModelAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(AZURE_MODEL_KEY);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string?> GetWeatherApiKeyAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(WEATHER_API_KEY);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task StoreCredentialsAsync(string azureEndpoint, string azureApiKey, string azureModel, string weatherApiKey)
    {
        System.Diagnostics.Debug.WriteLine($"SecureCredentialService: Storing credentials - Endpoint: {azureEndpoint}, Model: {azureModel}");
        await SecureStorage.SetAsync(AZURE_ENDPOINT_KEY, azureEndpoint);
        await SecureStorage.SetAsync(AZURE_API_KEY, azureApiKey);
        await SecureStorage.SetAsync(AZURE_MODEL_KEY, azureModel);
        await SecureStorage.SetAsync(WEATHER_API_KEY, weatherApiKey);
        System.Diagnostics.Debug.WriteLine("SecureCredentialService: Credentials stored successfully");
    }

    public async Task ClearCredentialsAsync()
    {
        SecureStorage.Remove(AZURE_ENDPOINT_KEY);
        SecureStorage.Remove(AZURE_API_KEY);
        SecureStorage.Remove(AZURE_MODEL_KEY);
        SecureStorage.Remove(WEATHER_API_KEY);
        await Task.CompletedTask;
    }
}
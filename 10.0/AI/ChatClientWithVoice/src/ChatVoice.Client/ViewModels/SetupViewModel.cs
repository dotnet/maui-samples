using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatVoice.Client.Services;

namespace ChatVoice.Client.ViewModels;

public partial class SetupViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly ISecureCredentialService _credentialService;
    private readonly ChatViewModel _chatViewModel;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = "";

    [ObservableProperty]
    private bool hasCredentials;

    public SetupViewModel(IApiService apiService, ISecureCredentialService credentialService, ChatViewModel chatViewModel)
    {
        _apiService = apiService;
        _credentialService = credentialService;
        _chatViewModel = chatViewModel;

        _ = Task.Run(async () =>
        {
            HasCredentials = await _credentialService.HasCredentialsAsync();
        });
    }

    [RelayCommand]
    private async Task LoadCredentialsFromApi()
    {
        IsLoading = true;
        StatusMessage = "Fetching credentials from server...";
        try
        {
            var credentials = await _apiService.GetCredentialsAsync();
            if (credentials?.AzureOpenAI != null && !string.IsNullOrWhiteSpace(credentials.WeatherApiKey))
            {
                await _credentialService.StoreCredentialsAsync(
                    credentials.AzureOpenAI.Endpoint ?? string.Empty,
                    credentials.AzureOpenAI.ApiKey ?? string.Empty,
                    credentials.AzureOpenAI.Model ?? "gpt-4o-mini",
                    credentials.WeatherApiKey);
                HasCredentials = true;
                StatusMessage = "✅ Credentials loaded successfully!";
                await _chatViewModel.RefreshConfigurationCommand.ExecuteAsync(null);
            }
            else
            {
                StatusMessage = "❌ Invalid credentials received from server.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Failed to load credentials: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task ClearCredentials()
    {
        IsLoading = true;
        StatusMessage = "Clearing stored credentials...";
        try
        {
            await _credentialService.ClearCredentialsAsync();
            HasCredentials = false;
            StatusMessage = "Credentials cleared.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Failed to clear credentials: {ex.Message}";
        }
        finally { IsLoading = false; }
    }
}

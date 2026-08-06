using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using PasskeysApi = Microsoft.Maui.Authentication.Passkeys;

namespace Passkeys.Client.ViewModels;

public partial class PasskeysViewModel : ObservableObject
{
    readonly string serverBaseUrl = GetConfiguredServerUrl();
    HttpClient? httpClient;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string username = string.Empty;

    [ObservableProperty]
    string password = string.Empty;

    [ObservableProperty]
    string status = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoggedOut))]
    [NotifyPropertyChangedFor(nameof(AccountStatusText))]
    bool isSignedIn;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AccountStatusText))]
    string? currentUsername;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPasskey))]
    [NotifyPropertyChangedFor(nameof(PasskeyStatusText))]
    [NotifyPropertyChangedFor(nameof(CreatePasskeyButtonText))]
    int passkeyCount;

    public ObservableCollection<PasskeyItem> Passkeys { get; } = [];

    public bool IsSupported => PasskeysApi.IsSupported;

    public string SupportedText => IsSupported
        ? "Passkeys are supported on this device."
        : "Passkeys are not supported on this device or OS version.";

    public string ServerBaseUrl => serverBaseUrl;

    public bool IsNotBusy => !IsBusy;

    public bool IsLoggedOut => !IsSignedIn;

    public bool HasPasskey => PasskeyCount > 0;

    public string AccountStatusText => $"Signed in as {CurrentUsername}";

    public string PasskeyStatusText => HasPasskey
        ? PasskeyCount == 1
            ? "1 passkey is registered to this account."
            : $"{PasskeyCount} passkeys are registered to this account."
        : "No passkey is registered to this account.";

    public string CreatePasskeyButtonText => HasPasskey ? "Add another passkey" : "Create a passkey";

    [RelayCommand]
    async Task SignUpAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsBusy = true;
            Log($"Creating account '{Username}'...");

            await PostJsonAsync(
                "/account/register",
                new { email = Username, password = Password },
                cancellationToken);

            Log("Account created. Signing in...");
            await PostJsonAsync(
                "/account/login?useCookies=true",
                new { email = Username, password = Password },
                cancellationToken);

            await RefreshAfterSignInAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task SignInPasswordAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsBusy = true;
            Log($"Signing in as '{Username}'...");

            await PostJsonAsync(
                "/account/login?useCookies=true",
                new { email = Username, password = Password },
                cancellationToken);

            await RefreshAfterSignInAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void SignOut()
    {
        httpClient?.Dispose();
        httpClient = null;
        SetSignedOutState();
        Log("Signed out.");
    }

    [RelayCommand]
    async Task RegisterAsync(CancellationToken cancellationToken)
    {
        if (!EnsureSupported())
        {
            return;
        }

        try
        {
            IsBusy = true;
            Log("Requesting creation options...");
            var creationOptionsJson = await PostAsync(
                "/passkeys/register/begin",
                cancellationToken: cancellationToken);

            Log("Creating a passkey with the platform authenticator...");
            var response = await PasskeysApi.CreateAsync(creationOptionsJson, cancellationToken);

            // The client does not verify the attestation. It sends the authenticator response to the
            // relying-party server, which validates the challenge, origin, RP ID, and attestation.
            Log("Sending the attestation to the server for verification...");
            var nameQuery = $"?name={Uri.EscapeDataString(BuildDeviceName())}";
            await PostAsync(
                $"/passkeys/register/finish{nameQuery}",
                response.ToString(),
                cancellationToken);

            await RefreshAccountStateAsync(cancellationToken);
            Log("Passkey created.");
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task LoginAsync(CancellationToken cancellationToken)
    {
        if (!EnsureSupported())
        {
            return;
        }

        try
        {
            IsBusy = true;
            Log("Requesting assertion options...");
            var requestOptionsJson = await PostAsync(
                "/passkeys/login/begin",
                cancellationToken: cancellationToken);

            Log("Asserting the passkey with the platform authenticator...");
            var response = await PasskeysApi.AssertAsync(requestOptionsJson, cancellationToken);

            // The native API obtains the assertion. Only the server verifies it and establishes
            // the authenticated session.
            Log("Sending the assertion to the server for verification...");
            await PostAsync(
                "/passkeys/login/finish",
                response.ToString(),
                cancellationToken);

            await RefreshAccountStateAsync(cancellationToken);
            Log($"Signed in with a passkey as {CurrentUsername}.");
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    async Task RefreshAfterSignInAsync(CancellationToken cancellationToken)
    {
        await RefreshAccountStateAsync(cancellationToken);
        if (IsSignedIn)
        {
            Log($"Signed in as {CurrentUsername}.");
        }
    }

    async Task RefreshAccountStateAsync(CancellationToken cancellationToken)
    {
        var client = GetClient();
        using var response = await client.GetAsync("/passkeys/list", cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            SetSignedOutState();
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Server returned {(int)response.StatusCode}: {ExtractServerMessage(body)}");
        }

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        CurrentUsername = root.TryGetProperty("username", out var usernameElement)
            ? usernameElement.GetString()
            : Username;
        PasskeyCount = root.TryGetProperty("passkeyCount", out var countElement)
            ? countElement.GetInt32()
            : 0;

        Passkeys.Clear();
        if (root.TryGetProperty("passkeys", out var list) &&
            list.ValueKind == JsonValueKind.Array)
        {
            foreach (var passkey in list.EnumerateArray())
            {
                Passkeys.Add(new PasskeyItem
                {
                    Id = passkey.TryGetProperty("id", out var id) ? id.GetString() : null,
                    Name = passkey.TryGetProperty("name", out var name) ? name.GetString() : null,
                    CreatedAt = passkey.TryGetProperty("createdAt", out var createdAt) &&
                                createdAt.TryGetDateTimeOffset(out var value)
                        ? value.ToLocalTime().ToString("MMM d, yyyy")
                        : null,
                });
            }
        }

        IsSignedIn = true;
    }

    void SetSignedOutState()
    {
        IsSignedIn = false;
        CurrentUsername = null;
        PasskeyCount = 0;
        Passkeys.Clear();
    }

    bool EnsureSupported()
    {
        OnPropertyChanged(nameof(IsSupported));
        OnPropertyChanged(nameof(SupportedText));
        if (PasskeysApi.IsSupported)
        {
            return true;
        }

        Log("Passkeys are not supported on this device or OS version.");
        return false;
    }

    async Task<string> PostAsync(
        string relativeUrl,
        string? jsonBody = null,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();
        using var content = new StringContent(jsonBody ?? string.Empty, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(relativeUrl, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Server returned {(int)response.StatusCode}: {ExtractServerMessage(body)}");
        }

        return body;
    }

    Task<string> PostJsonAsync(
        string relativeUrl,
        object payload,
        CancellationToken cancellationToken) =>
        PostAsync(relativeUrl, JsonSerializer.Serialize(payload), cancellationToken);

    HttpClient GetClient()
    {
        if (httpClient is null || httpClient.BaseAddress?.ToString() != NormalizeBaseUrl())
        {
            httpClient?.Dispose();
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
            };
            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(NormalizeBaseUrl()),
                Timeout = TimeSpan.FromMinutes(3),
            };
        }

        return httpClient;
    }

    string NormalizeBaseUrl()
    {
        var url = ServerBaseUrl.Trim();
        return url.EndsWith('/') ? url : $"{url}/";
    }

    static string GetConfiguredServerUrl()
    {
        foreach (var attribute in typeof(PasskeysViewModel)
                     .Assembly
                     .GetCustomAttributes<AssemblyMetadataAttribute>())
        {
            if (attribute.Key == "PasskeysServerUrl" &&
                !string.IsNullOrWhiteSpace(attribute.Value))
            {
                return attribute.Value;
            }
        }

        return "https://your-tunnel-5177.devtunnels.ms";
    }

    static string BuildDeviceName()
    {
        var name = string.IsNullOrWhiteSpace(DeviceInfo.Name)
            ? $"{DeviceInfo.Manufacturer} {DeviceInfo.Model}".Trim()
            : DeviceInfo.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Unknown device";
        }

        return $"{name} ({DeviceInfo.Platform} {DeviceInfo.VersionString})";
    }

    static string ExtractServerMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return "(no details)";
        }

        var trimmed = body.Trim();
        if (trimmed[0] == '{')
        {
            try
            {
                using var document = JsonDocument.Parse(trimmed);
                if (document.RootElement.TryGetProperty("error", out var error))
                {
                    return error.GetString() ?? trimmed;
                }

                if (document.RootElement.TryGetProperty("title", out var title))
                {
                    return title.GetString() ?? trimmed;
                }
            }
            catch (JsonException)
            {
            }
        }

        return trimmed;
    }

    void HandleError(Exception exception)
    {
        var message = exception switch
        {
            OperationCanceledException => "Canceled by the user.",
            HttpRequestException httpException =>
                $"Network error: {httpException.Message}. Is the server URL reachable?",
            _ => $"{exception.GetType().Name}: {exception.Message}",
        };

        Log(message);
    }

    void Log(string message) =>
        MainThread.BeginInvokeOnMainThread(() =>
            Status = $"{DateTime.Now:HH:mm:ss}  {message}");
}

public sealed class PasskeyItem
{
    public string? Id { get; init; }

    public string? Name { get; init; }

    public string? CreatedAt { get; init; }

    public string DisplayName => string.IsNullOrWhiteSpace(Name) ? ShortId : Name;

    public string ShortId => string.IsNullOrEmpty(Id)
        ? "(unknown id)"
        : Id.Length <= 16
            ? Id
            : $"{Id[..16]}...";

    public string CreatedAtText => string.IsNullOrEmpty(CreatedAt)
        ? string.Empty
        : $"Added {CreatedAt}";
}

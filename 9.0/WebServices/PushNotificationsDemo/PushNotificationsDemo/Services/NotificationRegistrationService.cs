using System.Text;
using System.Text.Json;
using PushNotificationsDemo.Models;

namespace PushNotificationsDemo.Services;

public class NotificationRegistrationService : INotificationRegistrationService
{
    const string RequestUrl = "api/notifications/installations";
    const string CachedDeviceTokenKey = "cached_device_token";
    const string CachedTagsKey = "cached_tags";

    string _baseApiUrl;
    HttpClient _client;
    IDeviceInstallationService _deviceInstallationService;

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ?? (_deviceInstallationService = Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IDeviceInstallationService>());

    public NotificationRegistrationService(string baseApiUri, string apiKey)
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("apikey", apiKey);

        _baseApiUrl = baseApiUri;
    }

    public async Task DeregisterDeviceAsync()
    {
        var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
            .ConfigureAwait(false);

        if (cachedToken == null)
            return;

        var deviceId = DeviceInstallationService?.GetDeviceId();

        if (string.IsNullOrWhiteSpace(deviceId))
            throw new Exception("Unable to resolve an ID for the device.");

        await SendAsync(HttpMethod.Delete, $"{RequestUrl}/{deviceId}")
            .ConfigureAwait(false);

        SecureStorage.Remove(CachedDeviceTokenKey);
        SecureStorage.Remove(CachedTagsKey);
    }

    public async Task RegisterDeviceAsync(params string[] tags)
    {
        var deviceInstallation = DeviceInstallationService?.GetDeviceInstallation(tags);

        await SendAsync<DeviceInstallation>(HttpMethod.Put, RequestUrl, deviceInstallation)
            .ConfigureAwait(false);

        await SecureStorage.SetAsync(CachedDeviceTokenKey, deviceInstallation.PushChannel)
            .ConfigureAwait(false);

        await SecureStorage.SetAsync(CachedTagsKey, JsonSerializer.Serialize(tags));
    }

    public async Task RefreshRegistrationAsync()
    {
        var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
            .ConfigureAwait(false);

        var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(cachedToken) ||
            string.IsNullOrWhiteSpace(serializedTags) ||
            string.IsNullOrWhiteSpace(_deviceInstallationService.Token) ||
            cachedToken == DeviceInstallationService.Token)
            return;

        var tags = JsonSerializer.Deserialize<string[]>(serializedTags);

        await RegisterDeviceAsync(tags);
    }

    async Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
    {
        string serializedContent = null;

        await Task.Run(() => serializedContent = JsonSerializer.Serialize(obj))
            .ConfigureAwait(false);

        await SendAsync(requestType, requestUri, serializedContent);
    }

    async Task SendAsync(HttpMethod requestType, string requestUri, string jsonRequest = null)
    {
        var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));

        if (jsonRequest != null)
            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}

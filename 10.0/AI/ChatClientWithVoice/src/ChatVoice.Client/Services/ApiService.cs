using System.Net.Http.Json;

namespace ChatVoice.Client.Services;

public interface IApiService
{
    Task<CredentialResponse?> GetCredentialsAsync(CancellationToken ct = default);
}

public class ApiService(HttpClient http) : IApiService
{
    public async Task<CredentialResponse?> GetCredentialsAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<CredentialResponse>("api/credentials", ct);
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PushNotificationsAPI.Authentication;

public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthOptions>
{
    const string ApiKeyIdentifier = "apikey";

    public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string key = string.Empty;

        if (Request.Headers[ApiKeyIdentifier].Any())
        {
            key = Request.Headers[ApiKeyIdentifier].FirstOrDefault();
        }
        else if (Request.Query.ContainsKey(ApiKeyIdentifier))
        {
            if (Request.Query.TryGetValue(ApiKeyIdentifier, out var queryKey))
                key = queryKey;
        }

        if (string.IsNullOrWhiteSpace(key))
            return Task.FromResult(AuthenticateResult.Fail("No api key provided"));

        if (!string.Equals(key, Options.ApiKey, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.Fail("Invalid api key."));

        var identities = new List<ClaimsIdentity>
        {
            new ClaimsIdentity("ApiKeyIdentity")
        };

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

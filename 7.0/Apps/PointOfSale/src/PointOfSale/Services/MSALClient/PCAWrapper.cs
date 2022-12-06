// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Datasync.Client;
using PointOfSale.Data;

namespace PointOfSale.MSALClient;

/// <summary>
/// This is a wrapper for PCA. It is singleton and can be utilized by both application and the MAM callback
/// </summary>
public class PCAWrapper
{
    /// <summary>
    /// This is the singleton used by consumers
    /// </summary>
    public static PCAWrapper Instance { get; private set; } = new PCAWrapper();

    public IPublicClientApplication IdentityClient { get; set; }

    internal IPublicClientApplication PCA { get; }

    internal bool UseEmbedded { get; set; } = false;

    /// <summary>
    /// The authority for the MSAL PublicClientApplication. Sign in will use this URL.
    /// </summary>
    private const string _authority = "https://login.microsoftonline.com/common";

    public static string[] Scopes = { "User.Read" };

    // private constructor for singleton
    private PCAWrapper()
    {
        // Create PCA once. Make sure that all the config parameters below are passed
        PCA = PublicClientApplicationBuilder
                                    .Create(Constants.ApplicationId)
                                    .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
                                    .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                                    .Build();
    }

    /// <summary>
    /// Acquire the token silently
    /// </summary>
    /// <param name="scopes">desired scopes</param>
    /// <returns>Authentication result</returns>
    public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes)
    {
        var accts = await PCA.GetAccountsAsync().ConfigureAwait(false);
        var acct = accts.FirstOrDefault();

        if (acct != null)
        {
            var authResult = await PCA.AcquireTokenSilent(scopes, acct)
                                        .ExecuteAsync().ConfigureAwait(false);
            return authResult;
        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// Perform the interactive acquisition of the token for the given scope
    /// </summary>
    /// <param name="scopes">desired scopes</param>
    /// <returns></returns>
    internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
    {
        SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();
#if IOS
        // embedded view is not supported on Android
        if (UseEmbedded)
        {

            return await PCA.AcquireTokenInteractive(scopes)
                                    .WithUseEmbeddedWebView(true)
                                    .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                                    .ExecuteAsync()
                                    .ConfigureAwait(false);
        }

        // Hide the privacy prompt in iOS
        systemWebViewOptions.iOSHidePrivacyPrompt = true;
#endif

        return await PCA.AcquireTokenInteractive(scopes)
                                .WithSystemWebViewOptions(systemWebViewOptions)
                                .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                                .ExecuteAsync()
                                .ConfigureAwait(false);
    }

    /// <summary>
    /// Signout may not perform the complete signout as company portal may hold
    /// the token.
    /// </summary>
    /// <returns></returns>
    internal async Task SignOutAsync()
    {
        var accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
        foreach (var acct in accounts)
        {
            await PCA.RemoveAsync(acct).ConfigureAwait(false);
        }
    }

    public async Task<AuthenticationToken> GetAuthenticationToken(string[] scopes)
    {
        if (IdentityClient == null)
        {
#if ANDROID
            IdentityClient = PublicClientApplicationBuilder
                .Create(Constants.ApplicationId)
                .WithAuthority(AzureCloudInstance.AzurePublic, "common")
                .WithRedirectUri($"msal{Constants.ApplicationId}://auth")
                .WithParentActivityOrWindow(() => Platform.CurrentActivity)
                .Build();
#elif IOS
        IdentityClient = PublicClientApplicationBuilder
            .Create(Constants.ApplicationId)
            .WithAuthority(AzureCloudInstance.AzurePublic, "common")
            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            .WithRedirectUri($"msal{Constants.ApplicationId}://auth")
            .Build();
#else
        IdentityClient = PublicClientApplicationBuilder
            .Create(Constants.ApplicationId)
            .WithAuthority(AzureCloudInstance.AzurePublic, "common")
            .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
            .Build();
#endif
        }

        var accounts = await IdentityClient.GetAccountsAsync();
        AuthenticationResult result = null;
        bool tryInteractiveLogin = false;

        try
        {
            result = await IdentityClient
                .AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            tryInteractiveLogin = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
        }

        if (tryInteractiveLogin)
        {
            try
            {
                result = await IdentityClient
                    .AcquireTokenInteractive(Constants.Scopes)
                    .ExecuteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MSAL Interactive Error: {ex.Message}");
            }
        }

        return new AuthenticationToken
        {
            DisplayName = result?.Account?.Username ?? "",
            ExpiresOn = result?.ExpiresOn ?? DateTimeOffset.MinValue,
            Token = result?.AccessToken ?? "",
            UserId = result?.Account?.Username ?? ""
        };
    }
}

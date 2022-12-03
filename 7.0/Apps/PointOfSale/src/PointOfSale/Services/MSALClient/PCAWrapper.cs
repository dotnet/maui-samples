// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

        var authResult = await PCA.AcquireTokenSilent(scopes, acct)
                                    .ExecuteAsync().ConfigureAwait(false);
        return authResult;

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
}

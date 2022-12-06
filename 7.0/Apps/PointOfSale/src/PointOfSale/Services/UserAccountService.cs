
using MonkeyCache.FileStore;
using PointOfSale.Data;
using Microsoft.Identity.Client;
using Microsoft.Datasync.Client;

namespace PointOfSale.Services;

public class UserAccountService
{
    public UserAccountService()
    {
    }

    //public async Task<UserAccount> GetUserAccount()
    //{
    //    //Dev handle online/offline scenario
    //    //return Barrel.Current.Get<UserAccount>(key: "account");
    //    string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");

    //    if (oauthToken == null)
    //    {
    //        // No value is associated with the key "oauth_token"
    //    }
    //}

    //public async Task<bool> SetUserAccount(UserAccount ua)
    //{
    //    try
    //    {
    //        await SecureStorage.Default.SetAsync("oauth_token", "secret-oauth-token-value");
    //        //Barrel.Current.Add(key: "account", data: ua, expireIn: TimeSpan.FromDays(1));
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }

    //    return true;
    //}
}


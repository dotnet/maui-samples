using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace PointOfSale.Platforms.Android;

[Activity(Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
    DataHost = "auth",
    DataScheme = "msale8b7e84c-1cb6-4619-bee9-ace98d4211e5")]
public class MsalActivity : BrowserTabActivity
{
}
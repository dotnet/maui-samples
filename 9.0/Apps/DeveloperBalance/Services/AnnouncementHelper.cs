using System;
using DeveloperBalance;

namespace DeveloperBalance.Services;

public static class AnnouncementHelper
{
    public static async Task Announce(string recipeName)
    {
#if IOS
        var _semanticScreenReader = App.ServiceProvider.GetService<IAsyncAnnouncement>();
        await _semanticScreenReader.AnnounceAsync(recipeName);
#else
        SemanticScreenReader.Announce(recipeName);
#endif
    }
}

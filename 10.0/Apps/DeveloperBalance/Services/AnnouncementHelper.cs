using System;
using DeveloperBalance;

namespace DeveloperBalance.Services;

public static class AnnouncementHelper
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async Task Announce(string recipeName)
    {
#if IOS
        var _semanticScreenReader = App.ServiceProvider?.GetService<IAsyncAnnouncement>();
        if (_semanticScreenReader is not null)
        {
            await _semanticScreenReader.AnnounceAsync(recipeName);
        }
#else
        SemanticScreenReader.Announce(recipeName);
#endif
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

}

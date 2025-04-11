using System;

namespace DeveloperBalance.Services;

public interface IAsyncAnnouncement
{
    Task AnnounceAsync(string text);
}
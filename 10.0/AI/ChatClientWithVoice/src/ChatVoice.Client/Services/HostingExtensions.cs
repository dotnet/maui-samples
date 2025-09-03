using Microsoft.Extensions.DependencyInjection;

namespace ChatVoice.Client.Services;

public static class HostingExtensions
{
    public static MauiAppBuilder AddChatClientWithTools(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<IChatService, ChatService>();
        return builder;
    }
}

using Microsoft.Maui.Handlers;

#if ANDROID
using PlatformWindow = Android.App.Activity;
#elif IOS || MACCATALYST
using PlatformWindow = UIKit.UIWindow;
#elif WINDOWS
using PlatformWindow = Microsoft.UI.Xaml.Window;
#endif

namespace Microsoft.Maui.Controls;

internal class EmbeddedWindowHandler : ElementHandler<IWindow, PlatformWindow>, IWindowHandler
{
    public static IPropertyMapper<IWindow, IWindowHandler> Mapper =
        new PropertyMapper<IWindow, IWindowHandler>(ElementHandler.ElementMapper)
        {
        };

    public static CommandMapper<IWindow, IWindowHandler> CommandMapper =
        new CommandMapper<IWindow, IWindowHandler>(ElementHandler.ElementCommandMapper)
        {
        };

    public EmbeddedWindowHandler() : base(Mapper)
    {
    }

    protected override PlatformWindow CreatePlatformElement() =>
        MauiContext!.Services.GetRequiredService<PlatformWindow>() ??
        throw new InvalidOperationException("EmbeddedWindowHandler could not locate a platform window.");
}
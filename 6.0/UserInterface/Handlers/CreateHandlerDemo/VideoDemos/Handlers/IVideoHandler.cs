#if IOS || MACCATALYST
using PlatformView = VideoDemos.Platforms.MaciOS.MauiVideoPlayer;
#elif ANDROID
using PlatformView = VideoDemos.Platforms.Android.MauiVideoPlayer;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0 && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif
using VideoDemos.Controls;

namespace VideoDemos.Handlers
{
    public interface IVideoHandler : IViewHandler
    {
        new IVideo VirtualView { get; }
        new PlatformView PlatformView { get; }
    }
}

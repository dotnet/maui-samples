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
using Microsoft.Maui.Handlers;

namespace VideoDemos.Handlers
{
    public partial class VideoHandler : IVideoHandler
    {
        public static IPropertyMapper<IVideo, IVideoHandler> PropertyMapper = new PropertyMapper<IVideo, IVideoHandler>(ViewHandler.ViewMapper)
        {
            [nameof(IVideo.AreTransportControlsEnabled)] = MapAreTransportControlsEnabled,
            [nameof(IVideo.Source)] = MapSource,
            [nameof(IVideo.Position)] = MapPosition
        };

        public static CommandMapper<IVideo, IVideoHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(IVideo.UpdateStatus)] = MapUpdateStatus,
            [nameof(IVideo.PlayRequested)] = MapPlayRequested,
            [nameof(IVideo.PauseRequested)] = MapPauseRequested,
            [nameof(IVideo.StopRequested)] = MapStopRequested
        };

        IVideo IVideoHandler.VirtualView => VirtualView;

        PlatformView IVideoHandler.PlatformView => PlatformView;

        public VideoHandler() : base(PropertyMapper, CommandMapper)
        {
        }
    }
}

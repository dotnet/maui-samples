#nullable enable
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using VideoDemos.Controls;

namespace VideoDemos.Handlers
{
    public partial class VideoHandler : ViewHandler<IVideo, FrameworkElement>
    {
        protected override FrameworkElement CreatePlatformView() => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapAreTransportControlsEnabled(IVideoHandler handler, IVideo video) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapSource(IVideoHandler handler, IVideo video) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapPosition(IVideoHandler handler, IVideo video) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapUpdateStatus(IVideoHandler handler, IVideo video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapPlayRequested(IVideoHandler handler, IVideo video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapPauseRequested(IVideoHandler handler, IVideo video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapStopRequested(IVideoHandler handler, IVideo video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
    }
}

#nullable enable
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using VideoDemos.Controls;

namespace VideoDemos.Handlers
{
    public partial class VideoHandler : ViewHandler<Video, FrameworkElement>
    {
        protected override FrameworkElement CreatePlatformView() => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapAreTransportControlsEnabled(VideoHandler handler, Video video) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapSource(VideoHandler handler, Video video) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapPosition(VideoHandler handler, Video video) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapUpdateStatus(VideoHandler handler, Video video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapPlayRequested(VideoHandler handler, Video video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapPauseRequested(VideoHandler handler, Video video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
        public static void MapStopRequested(VideoHandler handler, Video video, object? arg) => throw new PlatformNotSupportedException("No MediaElement control on Windows.");
    }
}

#nullable enable
using Microsoft.Maui.Handlers;
using VideoDemos.Controls;
using VideoDemos.Platforms.Android;

namespace VideoDemos.Handlers
{
    public partial class VideoHandler : ViewHandler<IVideo, MauiVideoPlayer>
    {
        protected override MauiVideoPlayer CreatePlatformView() => new MauiVideoPlayer(Context, VirtualView);

        protected override void ConnectHandler(MauiVideoPlayer platformView)
        {
            base.ConnectHandler(platformView);

            // Perform any control setup here
        }

        protected override void DisconnectHandler(MauiVideoPlayer platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        public static void MapAreTransportControlsEnabled(IVideoHandler handler, IVideo video)
        {
            handler.PlatformView?.UpdateTransportControlsEnabled();
        }

        public static void MapSource(IVideoHandler handler, IVideo video)
        {
            handler.PlatformView?.UpdateSource();
        }

        public static void MapPosition(IVideoHandler handler, IVideo video)
        {
            handler.PlatformView?.UpdatePosition();
        }

        public static void MapUpdateStatus(IVideoHandler handler, IVideo video, object? args)
        {
            handler.PlatformView?.UpdateStatus();
        }

        public static void MapPlayRequested(IVideoHandler handler, IVideo video, object? args)
        {
            if (args is not VideoPositionEventArgs)
                return;

            TimeSpan position = ((VideoPositionEventArgs)args).Position;
            handler.PlatformView?.PlayRequested(position);
        }

        public static void MapPauseRequested(IVideoHandler handler, IVideo video, object? args)
        {
            if (args is not VideoPositionEventArgs)
                return;

            TimeSpan position = ((VideoPositionEventArgs)args).Position;
            handler.PlatformView?.PauseRequested(position);
        }

        public static void MapStopRequested(IVideoHandler handler, IVideo video, object? args)
        {
            if (args is not VideoPositionEventArgs)
                return;

            TimeSpan position = ((VideoPositionEventArgs)args).Position;
            handler.PlatformView?.StopRequested(position);
        }
    }
}

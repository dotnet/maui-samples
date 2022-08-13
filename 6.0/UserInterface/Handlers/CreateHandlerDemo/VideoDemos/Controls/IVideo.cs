namespace VideoDemos.Controls
{
    public interface IVideo : IView
    {
        bool AreTransportControlsEnabled { get; }
        VideoSource Source { get; }
        bool AutoPlay { get; }
        VideoStatus Status { get; }
        TimeSpan Duration { get; }
        TimeSpan Position { get; set; }
        TimeSpan TimeToEnd { get; }

        event EventHandler UpdateStatus;
        event EventHandler<VideoPositionEventArgs> PlayRequested;
        event EventHandler<VideoPositionEventArgs> PauseRequested;
        event EventHandler<VideoPositionEventArgs> StopRequested;
    }
}

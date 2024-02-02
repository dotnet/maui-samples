using Microsoft.UI.Xaml.Controls;
using VideoDemos.Controls;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Grid = Microsoft.UI.Xaml.Controls.Grid;

namespace VideoDemos.Platforms.Windows
{
    public class MauiVideoPlayer : Grid, IDisposable
    {
        MediaPlayerElement _mediaPlayerElement;
        Video _video;
        bool _isMediaPlayerAttached;

        public MauiVideoPlayer(Video video)
        {
            _video = video;
            _mediaPlayerElement = new MediaPlayerElement();
            this.Children.Add(_mediaPlayerElement);
        }

        public void Dispose()
        {
            if (_isMediaPlayerAttached)
            {
                _mediaPlayerElement.MediaPlayer.MediaOpened -= OnMediaPlayerMediaOpened;
                _mediaPlayerElement.MediaPlayer.Dispose();
            }
            _mediaPlayerElement = null;
        }

        public void UpdateTransportControlsEnabled()
        {
            _mediaPlayerElement.AreTransportControlsEnabled = _video.AreTransportControlsEnabled;
        }

        public async void UpdateSource()
        {
            bool hasSetSource = false;

            if (_video.Source is UriVideoSource)
            {
                string uri = (_video.Source as UriVideoSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(uri));
                    hasSetSource = true;
                }
            }
            else if (_video.Source is FileVideoSource)
            {
                string filename = (_video.Source as FileVideoSource).File;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filename);
                    _mediaPlayerElement.Source = MediaSource.CreateFromStorageFile(storageFile);
                    hasSetSource = true;
                }
            }
            else if (_video.Source is ResourceVideoSource)
            {
                string path = "ms-appx:///" + (_video.Source as ResourceVideoSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(path));
                    hasSetSource = true;
                }
            }

            if (hasSetSource && !_isMediaPlayerAttached)
            {
                _isMediaPlayerAttached = true;
                _mediaPlayerElement.MediaPlayer.MediaOpened += OnMediaPlayerMediaOpened;
            }

            if (hasSetSource && _video.AutoPlay)
            {
                _mediaPlayerElement.AutoPlay = true;
            }
        }

        public void UpdateIsLooping()
        {
            if (_isMediaPlayerAttached)
                _mediaPlayerElement.MediaPlayer.IsLoopingEnabled = _video.IsLooping;
        }

        public void UpdatePosition()
        {
            if (_isMediaPlayerAttached)
            {
                if (Math.Abs((_mediaPlayerElement.MediaPlayer.Position - _video.Position).TotalSeconds) > 1)
                {
                    _mediaPlayerElement.MediaPlayer.Position = _video.Position;
                }
            }
        }

        public void UpdateStatus()
        {
            if (_isMediaPlayerAttached)
            {
                VideoStatus status = VideoStatus.NotReady;

                switch (_mediaPlayerElement.MediaPlayer.CurrentState)
                {
                    case MediaPlayerState.Playing:
                        status = VideoStatus.Playing;
                        break;
                    case MediaPlayerState.Paused:
                    case MediaPlayerState.Stopped:
                        status = VideoStatus.Paused;
                        break;
                }

                ((IVideoController)_video).Status = status;
                _video.Position = _mediaPlayerElement.MediaPlayer.Position;
            }
        }

        public void PlayRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                _mediaPlayerElement.MediaPlayer.Play();
                System.Diagnostics.Debug.WriteLine($"Video playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void PauseRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                _mediaPlayerElement.MediaPlayer.Pause();
                System.Diagnostics.Debug.WriteLine($"Video paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void StopRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                // There's no Stop method so pause the video and reset its position
                _mediaPlayerElement.MediaPlayer.Pause();
                _mediaPlayerElement.MediaPlayer.Position = TimeSpan.Zero;
                System.Diagnostics.Debug.WriteLine($"Video stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        void OnMediaPlayerMediaOpened(MediaPlayer sender, object args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((IVideoController)_video).Duration = _mediaPlayerElement.MediaPlayer.NaturalDuration;
            });
        }
    }
}

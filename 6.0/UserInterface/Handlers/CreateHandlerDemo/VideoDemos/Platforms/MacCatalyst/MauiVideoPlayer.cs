using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;
using GameController;
using System.Diagnostics;
using UIKit;
using VideoDemos.Controls;

namespace VideoDemos.Platforms.MaciOS
{
    public class MauiVideoPlayer : UIView
    {
        AVPlayer _player;
        AVPlayerItem _playerItem;
        AVPlayerViewController _playerViewController;
        Video _video;
        NSObject? _playedToEndObserver;

        public MauiVideoPlayer(Video video)
        {
            _video = video;

            // Create AVPlayerViewController
            _playerViewController = new AVPlayerViewController();

            // Set Player property to AVPlayer
            _player = new AVPlayer();
            _playerViewController.Player = _player;

            // Use the View from the controller as the native control
            _playerViewController.View.Frame = this.Bounds;

            AddSubview(_playerViewController.View);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_player != null)
                {
                    DestroyPlayedToEndObserver();
                    _player.ReplaceCurrentItemWithPlayerItem(null);
                    _player.Dispose();
                }
                if (_playerViewController != null)
                    _playerViewController.Dispose();

                _video = null;
            }

            base.Dispose(disposing);
        }

        public void UpdateTransportControlsEnabled()
        {
            _playerViewController.ShowsPlaybackControls = _video.AreTransportControlsEnabled;
        }

        public void UpdateSource()
        {
            AVAsset asset = null;

            if (_video.Source is UriVideoSource)
            {
                string uri = (_video.Source as UriVideoSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                    asset = AVAsset.FromUrl(new NSUrl(uri));
            }
            else if (_video.Source is FileVideoSource)
            {
                string uri = (_video.Source as FileVideoSource).File;
                if (!string.IsNullOrWhiteSpace(uri))
                    asset = AVAsset.FromUrl(NSUrl.CreateFileUrl(new[] { uri }));
            }
            else if (_video.Source is ResourceVideoSource)
            {
                string path = (_video.Source as ResourceVideoSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string directory = Path.GetDirectoryName(path);
                    string filename = Path.GetFileNameWithoutExtension(path);
                    string extension = Path.GetExtension(path).Substring(1);
                    NSUrl url = NSBundle.MainBundle.GetUrlForResource(filename, extension, directory);
                    asset = AVAsset.FromUrl(url);
                }
            }

            if (asset != null)
                _playerItem = new AVPlayerItem(asset);
            else
                _playerItem = null;

            _player.ReplaceCurrentItemWithPlayerItem(_playerItem);
            if (_playerItem != null && _video.AutoPlay)
            {
                _player.Play();
            }
        }

        public void UpdateIsLooping()
        {
            DestroyPlayedToEndObserver();
            if (_video.IsLooping)
            {
                _player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
                _playedToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);
            }
            else
                _player.ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause;
        }

        public void UpdatePosition()
        {
            TimeSpan controlPosition = ConvertTime(_player.CurrentTime);
            if (Math.Abs((controlPosition - _video.Position).TotalSeconds) > 1)
            {
                _player.Seek(CMTime.FromSeconds(_video.Position.TotalSeconds, 1));
            }
        }

        public void UpdateStatus()
        {
            VideoStatus videoStatus = VideoStatus.NotReady;

            switch (_player.Status)
            {
                case AVPlayerStatus.ReadyToPlay:
                    switch (_player.TimeControlStatus)
                    {
                        case AVPlayerTimeControlStatus.Playing:
                            videoStatus = VideoStatus.Playing;
                            break;

                        case AVPlayerTimeControlStatus.Paused:
                            videoStatus = VideoStatus.Paused;
                            break;
                    }
                    break;
            }
            ((IVideoController)_video).Status = videoStatus;

            if (_playerItem != null)
            {
                ((IVideoController)_video).Duration = ConvertTime(_playerItem.Duration);
                _video.Position = ConvertTime(_playerItem.CurrentTime);
            }
        }

        public void PlayRequested(TimeSpan position)
        {
            _player.Play();
            Debug.WriteLine($"Video playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void PauseRequested(TimeSpan position)
        {
            _player.Pause();
            Debug.WriteLine($"Video paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void StopRequested(TimeSpan position)
        {
            _player.Pause();
            _player.Seek(new CMTime(0, 1));
            Debug.WriteLine($"Video stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        TimeSpan ConvertTime(CMTime cmTime)
        {
            return TimeSpan.FromSeconds(Double.IsNaN(cmTime.Seconds) ? 0 : cmTime.Seconds);
        }

        void PlayedToEnd(NSNotification notification)
        {
            if (_video == null || notification.Object != _playerViewController.Player?.CurrentItem)
                return;

            _playerViewController.Player?.Seek(CMTime.Zero);
        }

        void DestroyPlayedToEndObserver()
        {
            if (_playedToEndObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_playedToEndObserver);
                DisposeObserver(ref _playedToEndObserver);
            }
        }

        void DisposeObserver(ref NSObject? disposable)
        {
            disposable?.Dispose();
            disposable = null;
        }
    }
}

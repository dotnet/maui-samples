using VideoDemos.Controls;

namespace VideoDemos.Views;

public partial class CustomTransportPage : ContentPage
{
    public CustomTransportPage()
    {
        InitializeComponent();
    }

    void OnPlayPauseButtonClicked(object sender, EventArgs args)
    {
        if (video.Status == VideoStatus.Playing)
        {
            video.Pause();
        }
        else if (video.Status == VideoStatus.Paused)
        {
            video.Play();
        }
    }

    void OnStopButtonClicked(object sender, EventArgs args)
    {
        video.Stop();
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }
}
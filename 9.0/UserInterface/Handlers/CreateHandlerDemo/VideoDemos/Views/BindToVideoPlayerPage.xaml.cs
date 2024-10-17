namespace VideoDemos.Views;

public partial class BindToVideoPlayerPage : ContentPage
{
    public BindToVideoPlayerPage()
    {
        InitializeComponent();
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }
}
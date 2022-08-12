namespace VideoDemos.Views;

public partial class PlayWebVideoPage : ContentPage
{
    public PlayWebVideoPage()
    {
        InitializeComponent();
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }
}
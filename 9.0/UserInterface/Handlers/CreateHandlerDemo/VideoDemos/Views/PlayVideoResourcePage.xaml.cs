namespace VideoDemos.Views;

public partial class PlayVideoResourcePage : ContentPage
{
    public PlayVideoResourcePage()
    {
        InitializeComponent();
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }

}
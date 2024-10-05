using VideoDemos.Controls;

namespace VideoDemos.Views;

public partial class PlayLibraryVideoPage : ContentPage
{
    public PlayLibraryVideoPage()
    {
        InitializeComponent();
    }

    async void OnShowVideoLibraryClicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        button.IsEnabled = false;

        var pickedVideo = await MediaPicker.PickVideoAsync();
        if (!string.IsNullOrWhiteSpace(pickedVideo?.FileName))
        {
            video.Source = new FileVideoSource
            {
                File = pickedVideo.FullPath
            };
        }

        button.IsEnabled = true;
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }
}
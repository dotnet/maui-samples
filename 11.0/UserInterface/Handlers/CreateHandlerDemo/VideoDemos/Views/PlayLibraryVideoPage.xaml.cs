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

        var pickedVideos = await MediaPicker.PickVideosAsync(new MediaPickerOptions { SelectionLimit = 1 });
        var pickedVideo = pickedVideos.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(pickedVideo?.FileName))
        {
            video.Source = new FileVideoSource
            {
                File = pickedVideo.FullPath
            };
        }

        button.IsEnabled = true;
    }
}
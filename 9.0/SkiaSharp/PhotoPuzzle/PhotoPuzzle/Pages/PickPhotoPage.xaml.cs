using SkiaSharp;

namespace PhotoPuzzle;

public partial class PickPhotoPage : ContentPage
{
	public PickPhotoPage()
	{
		InitializeComponent();
	}

    async void OnPickButtonClicked(object sender, EventArgs args)
    {
        // Load bitmap from photo library
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo != null)
        {
            using (Stream stream = await photo.OpenReadAsync())
            {
                if (stream != null)
                {
                    SKBitmap bitmap = SKBitmap.Decode(stream);
                    await Navigation.PushAsync(new RotatePhotoPage(bitmap));
                }
            }
        }
    }
}

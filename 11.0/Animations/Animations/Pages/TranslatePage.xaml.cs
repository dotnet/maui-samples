namespace Animations;

public partial class TranslatePage : ContentPage
{
	public TranslatePage()
	{
		InitializeComponent();
	}

	private async void OnClickedAsync(object sender, EventArgs e)
	{
		// distance to right edge
		var rightDist = (DeviceDisplay.MainDisplayInfo.Width - BotImg.X)/DeviceDisplay.MainDisplayInfo.Density;
		
		await Task.WhenAll(
            BotImg.TranslateToAsync(rightDist,0, 1000, easing: Easing.CubicInOut),
			BotImg.ScaleToAsync(0.4, 1000, easing: Easing.CubicInOut)
        );

        BotImg.TranslationX = rightDist*-1;

        await Task.WhenAll(
            BotImg.TranslateToAsync(0, 0, 1000, easing: Easing.CubicInOut),
            BotImg.ScaleToAsync(1, 1000, easing: Easing.CubicInOut)
        );

        BotImg.TranslationX = 0;
		BotImg.Scale = 1;
        
	}
}


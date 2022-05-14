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
            BotImg.TranslateTo(rightDist,0, 1000, easing: Easing.CubicInOut),
			BotImg.ScaleTo(0.4, 1000, easing: Easing.CubicInOut)
        );

        BotImg.TranslationX = rightDist*-1;

        await Task.WhenAll(
            BotImg.TranslateTo(0, 0, 1000, easing: Easing.CubicInOut),
            BotImg.ScaleTo(1, 1000, easing: Easing.CubicInOut)
        );

        BotImg.TranslationX = 0;
		BotImg.Scale = 1;
        
	}
}


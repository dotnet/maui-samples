namespace Animations;

public partial class RotatePage : ContentPage
{
	public RotatePage()
	{
		InitializeComponent();
	}

	private async void OnClickedAsync(object sender, EventArgs e)
	{
		await Task.WhenAny<bool>(
            BotImg.RotateToAsync(360, 500, Easing.CubicInOut),
			BotImg.TranslateToAsync(0,-50, 250, Easing.CubicInOut)
        );
        await BotImg.TranslateToAsync(0, 0, 250, Easing.CubicInOut);
        BotImg.Rotation = 0;
	}
}


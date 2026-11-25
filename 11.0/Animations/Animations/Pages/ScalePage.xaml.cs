namespace Animations;

public partial class ScalePage : ContentPage
{
	public ScalePage()
	{
		InitializeComponent();
	}

	private async void OnClickedAsync(object sender, EventArgs e)
	{
		await BotImg.ScaleToAsync(0, easing:Easing.SpringIn);
        await Task.Delay(1000);
        await BotImg.ScaleToAsync(1, easing: Easing.SpringOut);
	}
}


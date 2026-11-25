namespace Animations;

public partial class FadePage : ContentPage
{
	public FadePage()
	{
		InitializeComponent();
	}

	private async void OnClickedAsync(object sender, EventArgs e)
	{
		await BotImg.FadeToAsync(0);
        await Task.Delay(1000);
        await BotImg.FadeToAsync(1);
	}
}


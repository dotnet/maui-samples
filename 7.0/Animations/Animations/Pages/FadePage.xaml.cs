namespace Animations;

public partial class FadePage : ContentPage
{
	public FadePage()
	{
		InitializeComponent();
	}

	private async void OnClickedAsync(object sender, EventArgs e)
	{
		await BotImg.FadeTo(0);
        await Task.Delay(1000);
        await BotImg.FadeTo(1);
	}
}


using Animations.Extensions;

namespace Animations;

public partial class FadePage : ContentPage
{
	public FadePage()
	{
		InitializeComponent();
	}

    private async void OnClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Watch the bot fade away and return";
        await BotImg.FadeTo(0);
        await Task.Delay(1000);
        await BotImg.FadeTo(1);

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnGhostClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Wooooo! Ghost bot coming through!";

        for (int i = 0; i < 3; i++)
        {
            await BotImg.FadeTo(0.2, 500, Easing.SinInOut);
            await BotImg.FadeTo(0.8, 500, Easing.SinInOut);
        }
        await BotImg.FadeTo(1, 500, Easing.SinInOut);

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }
    private async void OnPulseClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Feel the pulse of the bot!";

        for (int i = 0; i < 3; i++)
        {
            await Task.WhenAll(
                BotImg.FadeTo(0.5, 300, Easing.CubicInOut),
                BotImg.ScaleTo(1.2, 300, Easing.CubicInOut)
            );
            await Task.WhenAll(
                BotImg.FadeTo(1, 300, Easing.CubicInOut),
                BotImg.ScaleTo(1, 300, Easing.CubicInOut)
            );
        }

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnDigitalGlitchClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Experiencing digital interference...";

        for (int i = 0; i < 5; i++)
        {
            await BotImg.FadeTo(0.2, 50, Easing.Linear);
            await BotImg.FadeTo(1, 100, Easing.Linear);
            await BotImg.FadeTo(0.5, 50, Easing.Linear);
            await BotImg.FadeTo(0.8, 75, Easing.Linear);
        }

        await BotImg.FadeTo(0, 500, Easing.CubicIn);
        await Task.Delay(300);
        await BotImg.FadeTo(1, 1000, Easing.CubicOut);

        await Task.Delay(500);
        TooltipLabel.Text = "Click any button to see what it does!";
    }
}


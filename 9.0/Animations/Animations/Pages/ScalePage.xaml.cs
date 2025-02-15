namespace Animations;

public partial class ScalePage : ContentPage
{
	public ScalePage()
	{
		InitializeComponent();
	}

    private async void OnClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Shrinks the dot net bot so it seems to disappear";
        await BotImg.ScaleTo(0, easing: Easing.SpringIn);
        await Task.Delay(1000);
        await BotImg.ScaleTo(1, easing: Easing.SpringOut);
    }

    private async void OnSquashStretchClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Makes the bot squash and stretch like rubber";
        await Task.WhenAll(
            BotImg.ScaleYTo(2, 500, Easing.SpringOut),
            BotImg.ScaleXTo(0.5, 500, Easing.SpringOut)
        );

        await Task.WhenAll(
            BotImg.ScaleYTo(1, 500, Easing.SpringIn),
            BotImg.ScaleXTo(1, 500, Easing.SpringIn)
        );
    }

    private async void OnRelativeScaleClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Scales the bot relative to current size";
        await BotImg.RelScaleTo(0.5, 500, Easing.SpringIn);
        await Task.Delay(500);
        await BotImg.RelScaleTo(-0.5, 500, Easing.SpringOut);
    }

    private async void OnAnchoredScaleClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Scales the bot from bottom center";
        BotImg.AnchorX = 0.5;
        BotImg.AnchorY = 1.0;

        await BotImg.ScaleTo(2, 500, Easing.CubicInOut);
        await Task.Delay(500);
        await BotImg.ScaleTo(1, 500, Easing.CubicInOut);

        BotImg.AnchorX = 0.5;
        BotImg.AnchorY = 0.5;
    }
}


using Animations.Extensions;

namespace Animations;

public partial class CustomAnimationPage : ContentPage
{
	public CustomAnimationPage()
	{
		InitializeComponent();
	}

    private async void OnClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Changing the page background color randomly";
        Color bgColor = this.BackgroundColor;
        await Task.WhenAll(
            this.ColorTo(bgColor, GetRandomColour(), c => this.BackgroundColor = c)
        );

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnColorWaveClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Watch the colorful wave effect!";

        var animation = new Animation();
        animation.Add(0, 1, new Animation(v => BotImg.Scale = 1 + Math.Sin(v * Math.PI) * 0.2, 0, 1));

        animation.Commit(this, "ColorWave", 16, 2000, Easing.SinInOut,
            (v, c) => BotImg.Scale = 1);

        await this.ColorTo(Colors.Purple, Colors.Orange, c => BackgroundColor = c, 2000, Easing.SinInOut);

        await Task.Delay(500);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnDancePartyClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "It's dance time!";

        for (int i = 0; i < 3; i++)
        {
            await Task.WhenAll(
                BotImg.RotateTo(360, 500, Easing.CubicInOut),
                BotImg.TranslateTo(50, 0, 250, Easing.CubicOut),
                BotImg.ScaleTo(1.2, 250, Easing.SpringOut)
            );

            await Task.WhenAll(
                BotImg.TranslateTo(-50, 0, 500, Easing.CubicOut),
                BotImg.ScaleTo(0.8, 250, Easing.SpringOut)
            );

            await Task.WhenAll(
                BotImg.TranslateTo(0, 0, 250, Easing.CubicOut),
                BotImg.ScaleTo(1, 250, Easing.SpringOut)
            );
        }

        BotImg.Rotation = 0;
        await Task.Delay(500);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnSpinClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Entering the third dimension!";

        await Task.WhenAll(
            BotImg.RotateYTo(1800, 2000, Easing.CubicInOut),
            BotImg.ScaleTo(0.5, 1000, Easing.CubicIn),
            BotImg.ScaleTo(1, 1000, Easing.CubicOut)
        );

        BotImg.RotationY = 0;
        await Task.Delay(500);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private static readonly Random rand = new Random();

    private Color GetRandomColour()
    {
        return Color.FromRgb(rand.Next(256), rand.Next(256), rand.Next(256));
    }
}
namespace Animations;

public partial class RotatePage : ContentPage
{
	public RotatePage()
	{
		InitializeComponent();
	}

    private async void OnClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Flips the dot net bot with a jump effect";
        await Task.WhenAny<bool>(
            BotImg.RotateTo(360, 500, Easing.CubicInOut),
            BotImg.TranslateTo(0, -50, 250, Easing.CubicInOut)
        );
        await BotImg.TranslateTo(0, 0, 250, Easing.CubicInOut);
        BotImg.Rotation = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnRotateXClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Rotates the bot on its X axis (vertical flip)";
        await BotImg.RotateXTo(360, 1000, Easing.CubicInOut);
        BotImg.RotationX = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnRotateYClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Rotates the bot on its Y axis (horizontal flip)";
        await BotImg.RotateYTo(360, 1000, Easing.CubicInOut);
        BotImg.RotationY = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnPropellerSpinClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Watch the bot spin like a propeller!";

        await BotImg.RotateTo(360, 1000, Easing.CubicIn);

        for (int i = 0; i < 3; i++)
        {
            await Task.WhenAll(
                BotImg.RotateTo(BotImg.Rotation + 720, 500, Easing.Linear),
                BotImg.RotateYTo(45, 250, Easing.CubicOut),
                BotImg.RotateYTo(-45, 250, Easing.CubicIn)
            );
        }

        await Task.WhenAll(
            BotImg.RotateTo(BotImg.Rotation + 360, 1000, Easing.CubicOut),
            BotImg.RotateYTo(0, 500, Easing.SpringOut)
        );

        BotImg.Rotation = 0;
        BotImg.RotationY = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }
}


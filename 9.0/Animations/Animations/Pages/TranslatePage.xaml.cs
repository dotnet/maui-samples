namespace Animations;

public partial class TranslatePage : ContentPage
{
	public TranslatePage()
	{
		InitializeComponent();
	}

	private async void OnClickedAsync(object sender, EventArgs e)
	{
        TooltipLabel.Text = "Watch the bot run to the edge and back!";
        // distance to right edge
        var rightDist = (DeviceDisplay.MainDisplayInfo.Width - BotImg.X)/DeviceDisplay.MainDisplayInfo.Density;

        await Task.WhenAll(
        BotImg.TranslateTo(rightDist, 0, 1000, easing: Easing.CubicInOut),
        BotImg.ScaleTo(0.4, 1000, easing: Easing.CubicInOut)
    );

        BotImg.TranslationX = rightDist * -1;

        await Task.WhenAll(
            BotImg.TranslateTo(0, 0, 1000, easing: Easing.CubicInOut),
            BotImg.ScaleTo(1, 1000, easing: Easing.CubicInOut)
        );

        BotImg.TranslationX = 0;
        BotImg.Scale = 1;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnBounceClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Watch the bot bounce up and down!";

        // Bounce animation sequence
        for (int i = 0; i < 3; i++)
        {
            await BotImg.TranslateTo(0, -50, 250, Easing.SpringOut);
            await BotImg.TranslateTo(0, 0, 250, Easing.SpringIn);
        }

        BotImg.TranslationY = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnDiagonalClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Watch the bot dance diagonally!";

        // Calculate diagonal distance based on screen size
        var diagDist = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) * 0.3;

        // Diagonal movement pattern
        await BotImg.TranslateTo(diagDist, -diagDist, 500, Easing.CubicInOut);
        await BotImg.TranslateTo(-diagDist, -diagDist, 500, Easing.CubicInOut);
        await BotImg.TranslateTo(-diagDist, diagDist, 500, Easing.CubicInOut);
        await BotImg.TranslateTo(0, 0, 500, Easing.CubicInOut);

        BotImg.TranslationX = 0;
        BotImg.TranslationY = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }

    private async void OnInfinityPathClickedAsync(object sender, EventArgs e)
    {
        TooltipLabel.Text = "Watch the bot trace an infinity pattern...";

        var width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) * 0.25;
        var height = width / 2;

        for (int i = 0; i < 2; i++)
        {
            await BotImg.TranslateTo(width / 2, -height, 500, Easing.CubicInOut);
            await BotImg.TranslateTo(width, 0, 500, Easing.CubicInOut);
            await BotImg.TranslateTo(width / 2, height, 500, Easing.CubicInOut);
            await BotImg.TranslateTo(0, 0, 500, Easing.CubicInOut);

            await BotImg.TranslateTo(-width / 2, -height, 500, Easing.CubicInOut);
            await BotImg.TranslateTo(-width, 0, 500, Easing.CubicInOut);
            await BotImg.TranslateTo(-width / 2, height, 500, Easing.CubicInOut);
            await BotImg.TranslateTo(0, 0, 500, Easing.CubicInOut);
        }

        BotImg.TranslationX = 0;
        BotImg.TranslationY = 0;

        await Task.Delay(1000);
        TooltipLabel.Text = "Click any button to see what it does!";
    }
}


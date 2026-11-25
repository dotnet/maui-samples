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
		
		Color bgColor = this.BackgroundColor;
        await Task.WhenAll(
          this.ColorTo(bgColor, GetRandomColour(), c => this.BackgroundColor = c)
		);

		
        
    }

    private static readonly Random rand = new Random();

    private Color GetRandomColour()
    {
        return Color.FromRgb(rand.Next(256), rand.Next(256), rand.Next(256));
    }
}
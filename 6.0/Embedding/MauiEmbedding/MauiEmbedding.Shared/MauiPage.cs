namespace MauiEmbedding.Shared;

public class MauiPage : ContentPage
{
	public MauiPage()
	{
		Content = new Grid
		{
			BackgroundColor = Colors.Purple,
			Children = {
				new Label { 
					Text = "Welcome to .NET MAUI!",
					FontSize = 24,
					TextColor = Colors.White,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center
				}
			}
		};
	}
}
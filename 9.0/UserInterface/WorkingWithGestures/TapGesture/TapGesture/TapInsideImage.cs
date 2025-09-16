using System.Diagnostics;

namespace TapGesture
{
	public class TapInsideImage : ContentPage
	{
		int tapCount;
		readonly Label label;
		readonly Image image;

		public TapInsideImage()
		{
			BackgroundColor = Color.FromArgb("#F5F5F5");
			
			image = new Image
			{
				Source = "tapped.jpg",
				WidthRequest = 200,
				HeightRequest = 200,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};

			// Add shadow effect with a Border
			var imageBorder = new Border
			{
				Content = image,
				StrokeThickness = 0,
				BackgroundColor = Colors.White,
				Shadow = new Shadow
				{
					Brush = Colors.Black,
					Opacity = 0.2f,
					Radius = 10,
					Offset = new Point(0, 5)
				},
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
			image.GestureRecognizers.Add(tapGestureRecognizer);

			label = new Label
			{
				Text = "Tap the photo to see the magic! ✨",
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.FromArgb("#2E2E2E"),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var titleLabel = new Label
			{
				Text = "Image Tap Demo",
				FontSize = 28,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.FromArgb("#512BD4"),
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 20, 0, 10)
			};

			var instructionsLabel = new Label
			{
				Text = "Each tap alternates between color and black & white",
				FontSize = 16,
				TextColor = Color.FromArgb("#666666"),
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 0, 0, 30)
			};

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Padding = new Thickness(20, 50),
					Spacing = 20,
					Children =
					{
						titleLabel,
						instructionsLabel,
						imageBorder,
						label
					}
				}
			};
		}

		async void OnTapGestureRecognizerTapped(object? sender, TappedEventArgs e)
		{
			tapCount++;
			
			// Add a subtle animation
			await image.ScaleTo(0.9, 100);
			await image.ScaleTo(1.0, 100);
			
			label.Text = tapCount == 1 
				? "Great! You tapped it once 🎉" 
				: $"Awesome! {tapCount} taps so far! 🚀";

			var imageSender = (Image)sender;

			// watch the monkey go from color to black&white!
			if (tapCount % 2 == 0) {
				imageSender.Source = "tapped.jpg";
			} else {
				imageSender.Source = "tapped_bw.jpg";
			}
			Debug.WriteLine ("image tapped: " + ((FileImageSource)imageSender.Source).File);
		}
	}
}


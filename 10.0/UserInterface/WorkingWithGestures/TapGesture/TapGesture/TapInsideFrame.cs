using Microsoft.Maui.Controls.Shapes;

namespace TapGesture
{
	public class TapInsideFrame : ContentPage
	{
		int tapCount;
		readonly Label label;
		readonly Border border;

		public TapInsideFrame()
		{
			BackgroundColor = Color.FromArgb("#F8F9FA");
			
			border = new Border
			{
				Stroke = Color.FromArgb("#512BD4"),
				StrokeThickness = 3,
				BackgroundColor = Color.FromArgb("#E8F4FD"),
				Padding = new Thickness(30, 40),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				StrokeShape = new RoundRectangle
				{
					CornerRadius = new CornerRadius(15)
				},
				Shadow = new Shadow
				{
					Brush = Colors.Gray,
					Opacity = 0.3f,
					Radius = 8,
					Offset = new Point(0, 4)
				},
				Content = new Label
				{
					Text = "Tap Inside This Beautiful Border! 🎯",
					FontSize = 18,
					FontAttributes = FontAttributes.Bold,
					TextColor = Color.FromArgb("#512BD4"),
					HorizontalTextAlignment = TextAlignment.Center
				}
			};

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
			border.GestureRecognizers.Add(tapGestureRecognizer);

			label = new Label
			{
				Text = "Ready to tap? Go ahead! 👆",
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.FromArgb("#2E2E2E"),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var titleLabel = new Label
			{
				Text = "Border Tap Demo",
				FontSize = 28,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.FromArgb("#512BD4"),
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 20, 0, 10)
			};

			var instructionsLabel = new Label
			{
				Text = "Tap anywhere inside the bordered area to count taps",
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
					Spacing = 25,
					Children =
					{
						titleLabel,
						instructionsLabel,
						border,
						label
					}
				}
			};
		}

		async void OnTapGestureRecognizerTapped(object? sender, TappedEventArgs e)
		{
			tapCount++;
			
			// Add animation feedback
			await border.ScaleTo(0.95, 100);
			await border.ScaleTo(1.0, 100);
			
			// Change border color based on tap count
			var colors = new[] { "#512BD4", "#FF6B6B", "#4ECDC4", "#45B7D1", "#96CEB4", "#FFEAA7" };
			border.Stroke = Color.FromArgb(colors[tapCount % colors.Length]);
			
			label.Text = tapCount == 1 
				? "Perfect! First tap registered! 🎉" 
				: $"Fantastic! {tapCount} taps completed! 🌟";
		}
	}
}

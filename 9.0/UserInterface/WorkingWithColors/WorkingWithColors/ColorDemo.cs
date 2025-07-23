namespace WorkingWithColors;

public class ColorDemo : ContentPage
{
	public ColorDemo()
	{
		Title = "Color Demo";
		
		var red = new Label { Text = "Red", BackgroundColor = Colors.Red, TextColor = Colors.White, Padding = 10 };
		var orange = new Label { Text = "Orange", BackgroundColor = Color.FromArgb("#FF6A00"), TextColor = Colors.White, Padding = 10 };
		var yellow = new Label { Text = "Yellow", BackgroundColor = Color.FromHsla(0.167, 1.0, 0.5, 1.0), TextColor = Colors.Black, Padding = 10 };
		var green = new Label { Text = "Green", BackgroundColor = Color.FromRgb(38, 127, 0), TextColor = Colors.White, Padding = 10 };
		var blue = new Label { Text = "Blue", BackgroundColor = Color.FromRgba(0, 38, 255, 255), TextColor = Colors.White, Padding = 10 };
		var indigo = new Label { Text = "Indigo", BackgroundColor = Color.FromRgb(0, 72, 255), TextColor = Colors.White, Padding = 10 };
		var violet = new Label { Text = "Violet", BackgroundColor = Color.FromHsla(0.82, 1, 0.25, 1), TextColor = Colors.White, Padding = 10 };

		var transparent = new Label { Text = "Transparent", BackgroundColor = Colors.Transparent, TextColor = Colors.Black, Padding = 10 };
		var defaultColor = new Label { Text = "Default (Transparent)", BackgroundColor = Colors.Transparent, TextColor = Colors.Black, Padding = 10 };

		var primary = new Label {
			Text = "Primary (App Primary Color)",
			BackgroundColor = (Color)Application.Current.Resources["Primary"],
			TextColor = Colors.White,
			Padding = 10
		};
		var secondary = new Label {
			Text = "Secondary",
			BackgroundColor = (Color)Application.Current.Resources["Secondary"],
			TextColor = Colors.Black,
			Padding = 10
		};
		var tertiary = new Label {
			Text = "Tertiary",
			BackgroundColor = (Color)Application.Current.Resources["Tertiary"],
			TextColor = Colors.White,
			Padding = 10
		};
		var gray100 = new Label {
			Text = "Gray100",
			BackgroundColor = (Color)Application.Current.Resources["Gray100"],
			TextColor = Colors.Black,
			Padding = 10
		};
		var blueAccent = new Label {
			Text = "Blue100Accent",
			BackgroundColor = (Color)Application.Current.Resources["Blue100Accent"],
			TextColor = Colors.White,
			Padding = 10
		};

		Content = new ScrollView
		{
			Content = new StackLayout
			{
				Padding = new Thickness(20),
				Spacing = 10,
				Children = {
					new Label {
						Text = "Color Demo - Created in Code",
						FontSize = 20,
						FontAttributes = FontAttributes.Bold,
						HorizontalOptions = LayoutOptions.Center,
						Margin = new Thickness(0, 0, 0, 20)
					},
					new Label {
						Text = "Different ways to create colors:",
						FontSize = 16,
						FontAttributes = FontAttributes.Italic,
						HorizontalOptions = LayoutOptions.Center,
						Margin = new Thickness(0, 0, 0, 10)
					},
					red, orange, yellow, green, blue, indigo, violet,
					new Label {
						Text = "Special colors:",
						FontSize = 16,
						FontAttributes = FontAttributes.Italic,
						HorizontalOptions = LayoutOptions.Center,
						Margin = new Thickness(0, 10, 0, 10)
					},
					transparent, defaultColor, primary, secondary, tertiary, gray100, blueAccent
				}
			}
		};
	}
}

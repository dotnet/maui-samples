using Microsoft.Maui.Controls.Shapes;

namespace MauiEmbedding.Shared;

public class MauiControl : ContentView
{
	public MauiControl()
	{
        var grid = new Grid
        {
            RowDefinitions = {
                new RowDefinition{ Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition{ Height = new GridLength(1, GridUnitType.Auto) }
            },
            Background = Color.FromArgb("#f1f1f1"),
            Padding = new Thickness(8),
            RowSpacing = 8,
            HorizontalOptions = LayoutOptions.Center
        };

        grid.Add(
            new Rectangle { Fill = Color.FromArgb("#212121"), WidthRequest = 100, HeightRequest = 100, HorizontalOptions = LayoutOptions.Center },
            row: 0
        );

        grid.Add(
            new Label { Text = "Welcome to .NET MAUI!", HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromArgb("#212121") },
            row: 1
        );

        Content = grid;
	}
}
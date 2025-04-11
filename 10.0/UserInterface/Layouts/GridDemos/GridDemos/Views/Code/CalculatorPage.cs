﻿namespace GridDemos.Views.Code
{
    public class CalculatorPage : ContentPage
    {
        public CalculatorPage()
        {
            Style plainButton = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromArgb ("#eee") },
                    new Setter { Property = Button.TextColorProperty, Value = Colors.Black },
                    new Setter { Property = Button.CornerRadiusProperty, Value = 0 },
                    new Setter { Property = Button.FontSizeProperty, Value = 40 }
                }
            };

            Style darkerButton = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromArgb ("#ddd") },
                    new Setter { Property = Button.TextColorProperty, Value = Colors.Black },
                    new Setter { Property = Button.CornerRadiusProperty, Value = 0 },
                    new Setter { Property = Button.FontSizeProperty, Value = 40 }
                }
            };

            Style orangeButton = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromArgb ("#E8AD00") },
                    new Setter { Property = Button.TextColorProperty, Value = Colors.White },
                    new Setter { Property = Button.CornerRadiusProperty, Value = 0 },
                    new Setter { Property = Button.FontSizeProperty, Value = 40 }
                }
            };

            Grid grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(150) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Label label = new Label
            {
                Text = "0",
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.End,
                TextColor = Colors.White,
                FontSize = 60
            };

            grid.Add(label, 0, 0);
            Grid.SetColumnSpan(label, 4);

            grid.Add(new Button { Text = "C", Style = darkerButton }, 0, 1);
            grid.Add(new Button { Text = "+/-", Style = darkerButton }, 1, 1);
            grid.Add(new Button { Text = "%", Style = darkerButton }, 2, 1);
            grid.Add(new Button { Text = "div", Style = orangeButton }, 3, 1);
            grid.Add(new Button { Text = "7", Style = plainButton }, 0, 2);
            grid.Add(new Button { Text = "8", Style = plainButton }, 1, 2);
            grid.Add(new Button { Text = "9", Style = plainButton }, 2, 2);
            grid.Add(new Button { Text = "X", Style = orangeButton }, 3, 2);
            grid.Add(new Button { Text = "4", Style = plainButton }, 0, 3);
            grid.Add(new Button { Text = "5", Style = plainButton }, 1, 3);
            grid.Add(new Button { Text = "6", Style = plainButton }, 2, 3);
            grid.Add(new Button { Text = "-", Style = orangeButton }, 3, 3);
            grid.Add(new Button { Text = "1", Style = plainButton }, 0, 4);
            grid.Add(new Button { Text = "2", Style = plainButton }, 1, 4);
            grid.Add(new Button { Text = "3", Style = plainButton }, 2, 4);
            grid.Add(new Button { Text = "+", Style = orangeButton }, 3, 4);
            grid.Add(new Button { Text = ".", Style = plainButton }, 2, 5);
            grid.Add(new Button { Text = "=", Style = orangeButton }, 3, 5);

            Button zeroButton = new Button { Text = "0", Style = plainButton };
            grid.Add(zeroButton, 0, 5);
            Grid.SetColumnSpan(zeroButton, 2);

            Title = "Calculator layout demo";
            BackgroundColor = Color.FromArgb("#404040");
            Content = grid;
        }
    }
}


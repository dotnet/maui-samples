namespace SwipeGesture
{
    public class SwipeContainerPageCS : ContentPage
    {
        private Label resultLabel;
        private Label instructionLabel;

        public SwipeContainerPageCS()
        {
            Title = "Swipe Container (C#)";
            BackgroundColor = Color.FromArgb("#F5F5F5");
            
            CreateUI();
        }

        private void CreateUI()
        {
            // Header
            var headerFrame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 12,
                HasShadow = true,
                Padding = 20,
                Margin = new Thickness(20, 20, 20, 10)
            };

            var headerStack = new StackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = "🎪 Swipe Container Demo",
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#1C1C1C"),
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Enhanced container with visual feedback",
                        FontSize = 16,
                        TextColor = Color.FromArgb("#1C1C1C"),
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 5, 0, 0)
                    }
                }
            };

            headerFrame.Content = headerStack;

            // Instructions
            instructionLabel = new Label
            {
                Text = "👆 Swipe the colorful box in any direction",
                FontSize = 14,
                TextColor = Color.FromArgb("#1C1C1C"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 10)
            };

            // Create the swipe target with gradient background
            var gradientBoxView = new BoxView
            {
                WidthRequest = 280,
                HeightRequest = 280,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop { Color = Color.FromArgb("#667eea"), Offset = 0.0f },
                        new GradientStop { Color = Color.FromArgb("#764ba2"), Offset = 1.0f }
                    }
                }
            };

            // Direction indicators
            var directionGrid = new Grid
            {
                WidthRequest = 280,
                HeightRequest = 280,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            directionGrid.Children.Add(new Label
            {
                Text = "↑\nGreen",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            });

            directionGrid.Children.Add(new Label
            {
                Text = "↓\nRed",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            directionGrid.Children.Add(new Label
            {
                Text = "←\nBlue",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 0, 0, 0)
            });

            directionGrid.Children.Add(new Label
            {
                Text = "→\nOrange",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 20, 0)
            });

            // Stack the box and directions
            var swipeContentStack = new StackLayout
            {
                Children = { gradientBoxView, directionGrid }
            };

            // Create the enhanced swipe container
            var swipeContainer = new SwipeContainer
            {
                Content = swipeContentStack,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(20)
            };

            // Result label
            resultLabel = new Label
            {
                Text = "Ready to swipe! 🎯",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#6200EE"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 20, 20, 10)
            };

            // Result frame
            var resultFrame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 12,
                HasShadow = true,
                Padding = 20,
                Margin = new Thickness(20, 10, 20, 20),
                Content = resultLabel
            };

            swipeContainer.Swipe += OnSwipeDetected;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    BackgroundColor = Color.FromArgb("#F5F5F5"),
                    Children = {
                        headerFrame,
                        instructionLabel,
                        swipeContainer,
                        resultFrame
                    }
                }
            };
        }

        private void OnSwipeDetected(object sender, SwipedEventArgs e)
        {
            var direction = e.Direction.ToString();
            var emoji = GetDirectionEmoji(e.Direction);
            var color = GetDirectionColor(e.Direction);
            
            resultLabel.Text = $"{emoji} You swiped {direction}!";
            resultLabel.TextColor = color;
            
            // Update instruction
            instructionLabel.Text = $"Great! Try swiping in another direction";
        }

        private string GetDirectionEmoji(SwipeDirection direction)
        {
            return direction switch
            {
                SwipeDirection.Up => "⬆️",
                SwipeDirection.Down => "⬇️",
                SwipeDirection.Left => "⬅️",
                SwipeDirection.Right => "➡️",
                _ => "🎯"
            };
        }

        private Color GetDirectionColor(SwipeDirection direction)
        {
            return direction switch
            {
                SwipeDirection.Up => Color.FromArgb("#4CAF50"),    // Green
                SwipeDirection.Down => Color.FromArgb("#f44336"),  // Red
                SwipeDirection.Left => Color.FromArgb("#2196F3"),  // Blue
                SwipeDirection.Right => Color.FromArgb("#FF9800"), // Orange
                _ => Color.FromArgb("#6200EE")
            };
        }
    }
}

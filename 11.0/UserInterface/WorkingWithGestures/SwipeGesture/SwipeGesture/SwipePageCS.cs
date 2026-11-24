namespace SwipeGesture
{
    public class SwipePageCS : ContentPage
    {
        private Label resultLabel;
        private Border swipeFrame;
        private BoxView swipeBox;

        public SwipePageCS()
        {
            Title = "Swipe Page (C#)";
            BackgroundColor = Color.FromArgb("#F5F5F5");
            
            CreateUI();
        }

        private void CreateUI()
        {
            // Header
            var headerFrame = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                StrokeThickness = 0,
                Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.2f, Offset = new Point(0, 4), Radius = 8 },
                Padding = 20,
                Margin = new Thickness(20, 20, 20, 10)
            };

            var headerStack = new StackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = "🔄 Basic Swipe Demo",
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#1C1C1C"),
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Direct gesture recognizer implementation",
                        FontSize = 16,
                        TextColor = Color.FromArgb("#1C1C1C"),
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 5, 0, 0)
                    }
                }
            };

            headerFrame.Content = headerStack;

            // Instructions
            var instructionLabel = new Label
            {
                Text = "👆 Swipe the animated box in any direction",
                FontSize = 14,
                TextColor = Color.FromArgb("#1C1C1C"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 10)
            };

            // Create animated swipe box
            swipeBox = new BoxView
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
                        new GradientStop { Color = Color.FromArgb("#00c9ff"), Offset = 0.0f },
                        new GradientStop { Color = Color.FromArgb("#92fe9d"), Offset = 1.0f }
                    }
                }
            };

            // Create frame around the swipe box
            swipeFrame = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                StrokeThickness = 0,
                Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.2f, Offset = new Point(0, 4), Radius = 8 },
                Padding = 10,
                Margin = new Thickness(20),
                Content = swipeBox
            };

            // Add gesture recognizers
            AddGestureRecognizers();

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
            var resultFrame = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                StrokeThickness = 0,
                Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.2f, Offset = new Point(0, 4), Radius = 8 },
                Padding = 20,
                Margin = new Thickness(20, 10, 20, 20),
                Content = resultLabel
            };

            // Direction guide
            var guideFrame = new Border
            {
                BackgroundColor = Color.FromArgb("#E8F5E8"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                StrokeThickness = 0,
                                Padding = 15,
                Margin = new Thickness(20, 10, 20, 20)
            };

            var guideGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            guideGrid.Add(CreateDirectionGuide("↑", "Up", "#4CAF50"), 0, 0);
            guideGrid.Add(CreateDirectionGuide("↓", "Down", "#f44336"), 1, 0);
            guideGrid.Add(CreateDirectionGuide("←", "Left", "#2196F3"), 2, 0);
            guideGrid.Add(CreateDirectionGuide("→", "Right", "#FF9800"), 3, 0);

            guideFrame.Content = guideGrid;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    BackgroundColor = Color.FromArgb("#F5F5F5"),
                    Children = {
                        headerFrame,
                        instructionLabel,
                        swipeFrame,
                        resultFrame,
                        guideFrame
                    }
                }
            };
        }

        private StackLayout CreateDirectionGuide(string arrow, string direction, string colorHex)
        {
            return new StackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = arrow,
                        FontSize = 24,
                        TextColor = Color.FromArgb(colorHex),
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = direction,
                        FontSize = 12,
                        TextColor = Color.FromArgb(colorHex),
                        HorizontalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold
                    }
                }
            };
        }

        private void AddGestureRecognizers()
        {
            var leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
            leftSwipeGesture.Swiped += OnSwipeDetected;

            var rightSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
            rightSwipeGesture.Swiped += OnSwipeDetected;

            var upSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Up };
            upSwipeGesture.Swiped += OnSwipeDetected;

            var downSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Down };
            downSwipeGesture.Swiped += OnSwipeDetected;

            swipeBox.GestureRecognizers.Add(leftSwipeGesture);
            swipeBox.GestureRecognizers.Add(rightSwipeGesture);
            swipeBox.GestureRecognizers.Add(upSwipeGesture);
            swipeBox.GestureRecognizers.Add(downSwipeGesture);
        }

        private async void OnSwipeDetected(object sender, SwipedEventArgs e)
        {
            var direction = e.Direction.ToString();
            var emoji = GetDirectionEmoji(e.Direction);
            var color = GetDirectionColor(e.Direction);
            
            // Update result
            resultLabel.Text = $"{emoji} You swiped {direction}!";
            resultLabel.TextColor = color;
            
            // Animate the swipe
            await AnimateSwipe(e.Direction);
        }

        private async Task AnimateSwipe(SwipeDirection direction)
        {
            var originalColor = swipeFrame.BackgroundColor;
            var animationColor = GetDirectionColor(direction);

            // Flash animation
            await swipeFrame.FadeToAsync(0.7, 100);
            swipeFrame.BackgroundColor = animationColor;
            await swipeFrame.FadeToAsync(1, 100);
            
            // Movement animation
            double deltaX = 0, deltaY = 0;
            switch (direction)
            {
                case SwipeDirection.Left:
                    deltaX = -20;
                    break;
                case SwipeDirection.Right:
                    deltaX = 20;
                    break;
                case SwipeDirection.Up:
                    deltaY = -20;
                    break;
                case SwipeDirection.Down:
                    deltaY = 20;
                    break;
            }

            await swipeFrame.TranslateToAsync(deltaX, deltaY, 150);
            await swipeFrame.TranslateToAsync(0, 0, 150);
            
            // Restore original color
            swipeFrame.BackgroundColor = originalColor;
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

namespace SwipeGesture
{
    public class SwipeCommandPageCS : ContentPage
    {
        private Label resultLabel;
        private Frame swipeFrame;
        private BoxView swipeBox;

        public SwipeCommandPageCS()
        {
            Title = "Swipe Command (C#)";
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
                        Text = "⚡ Command Pattern Demo",
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#1C1C1C"),
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Using MVVM pattern with swipe commands",
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
                Text = "👆 Swipe the command-enabled box in any direction",
                FontSize = 14,
                TextColor = Color.FromArgb("#1C1C1C"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 10)
            };

            // Create the swipe box with rainbow gradient
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
                        new GradientStop { Color = Color.FromArgb("#fa709a"), Offset = 0.0f },
                        new GradientStop { Color = Color.FromArgb("#fee140"), Offset = 1.0f }
                    }
                }
            };

            // Create frame around the swipe box
            swipeFrame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 20,
                HasShadow = true,
                Padding = 10,
                Margin = new Thickness(20),
                Content = swipeBox
            };

            // Add command-based gesture recognizers
            AddCommandGestureRecognizers();

            // Result label with binding
            resultLabel = new Label
            {
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#6200EE"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 20, 20, 10)
            };

            resultLabel.SetBinding(Label.TextProperty, "SwipeDirection");

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

            // Command pattern info
            var infoFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#E3F2FD"),
                CornerRadius = 12,
                HasShadow = false,
                Padding = 15,
                Margin = new Thickness(20, 10, 20, 20)
            };

            var infoStack = new StackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = "💡 Command Pattern Benefits:",
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#1976D2"),
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0, 10)
                    },
                    new Label
                    {
                        Text = "• Decouples UI from business logic\n• Enables undo/redo functionality\n• Supports data binding\n• Testable and maintainable",
                        FontSize = 14,
                        TextColor = Color.FromArgb("#1976D2"),
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Start
                    }
                }
            };

            infoFrame.Content = infoStack;

            // Stats frame
            var statsFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#FFF3E0"),
                CornerRadius = 12,
                HasShadow = false,
                Padding = 15,
                Margin = new Thickness(20, 10, 20, 20)
            };

            var statsGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            statsGrid.Add(CreateStatItem("↑", "Up", "#4CAF50"), 0, 0);
            statsGrid.Add(CreateStatItem("↓", "Down", "#f44336"), 1, 0);
            statsGrid.Add(CreateStatItem("←", "Left", "#2196F3"), 2, 0);
            statsGrid.Add(CreateStatItem("→", "Right", "#FF9800"), 3, 0);

            statsFrame.Content = statsGrid;

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
                        infoFrame,
                        statsFrame
                    }
                }
            };

            // Set the ViewModel
            BindingContext = new SwipeCommandPageViewModel();
        }

        private StackLayout CreateStatItem(string arrow, string direction, string colorHex)
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

        private void AddCommandGestureRecognizers()
        {
            var leftSwipeGesture = new SwipeGestureRecognizer 
            { 
                Direction = SwipeDirection.Left, 
                CommandParameter = "Left" 
            };
            leftSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, "SwipeCommand");
            leftSwipeGesture.Swiped += OnSwipeForAnimation;

            var rightSwipeGesture = new SwipeGestureRecognizer 
            { 
                Direction = SwipeDirection.Right, 
                CommandParameter = "Right" 
            };
            rightSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, "SwipeCommand");
            rightSwipeGesture.Swiped += OnSwipeForAnimation;

            var upSwipeGesture = new SwipeGestureRecognizer 
            { 
                Direction = SwipeDirection.Up, 
                CommandParameter = "Up" 
            };
            upSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, "SwipeCommand");
            upSwipeGesture.Swiped += OnSwipeForAnimation;

            var downSwipeGesture = new SwipeGestureRecognizer 
            { 
                Direction = SwipeDirection.Down, 
                CommandParameter = "Down" 
            };
            downSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, "SwipeCommand");
            downSwipeGesture.Swiped += OnSwipeForAnimation;

            swipeBox.GestureRecognizers.Add(leftSwipeGesture);
            swipeBox.GestureRecognizers.Add(rightSwipeGesture);
            swipeBox.GestureRecognizers.Add(upSwipeGesture);
            swipeBox.GestureRecognizers.Add(downSwipeGesture);
        }

        private async void OnSwipeForAnimation(object sender, SwipedEventArgs e)
        {
            var color = GetDirectionColor(e.Direction);
            resultLabel.TextColor = color;
            
            // Animate the swipe
            await AnimateSwipe(e.Direction);
        }

        private async Task AnimateSwipe(SwipeDirection direction)
        {
            var originalColor = swipeFrame.BackgroundColor;
            var animationColor = GetDirectionColor(direction);

            // Pulse animation
            await swipeFrame.ScaleTo(1.1, 100);
            swipeFrame.BackgroundColor = animationColor;
            await swipeFrame.ScaleTo(1.0, 100);
            
            // Rotation animation
            await swipeFrame.RotateTo(direction == SwipeDirection.Left ? -5 : 
                                     direction == SwipeDirection.Right ? 5 : 0, 100);
            await swipeFrame.RotateTo(0, 100);
            
            // Restore original color
            swipeFrame.BackgroundColor = originalColor;
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

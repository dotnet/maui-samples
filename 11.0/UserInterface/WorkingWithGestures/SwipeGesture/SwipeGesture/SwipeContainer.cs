using System;
using Microsoft.Maui.Controls;

namespace SwipeGesture
{
    public class SwipeContainer : ContentView
    {
        public event EventHandler<SwipedEventArgs> Swipe;
        
        private Border containerFrame;
        private ContentView contentContainer;

        public SwipeContainer()
        {
            CreateUI();
            AddGestureRecognizers();
        }

        private void CreateUI()
        {
            containerFrame = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                StrokeThickness = 0,
                Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.2f, Offset = new Point(0, 4), Radius = 8 },
                Padding = 0,
                Stroke = Brush.Transparent
            };

            contentContainer = new ContentView
            {
                Padding = 10
            };

            containerFrame.Content = contentContainer;
            base.Content = containerFrame;
        }

        private void AddGestureRecognizers()
        {
            GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Left));
            GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Right));
            GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Up));
            GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Down));
        }

        public new View Content
        {
            get => contentContainer?.Content;
            set
            {
                if (contentContainer != null)
                    contentContainer.Content = value;
            }
        }

        SwipeGestureRecognizer GetSwipeGestureRecognizer(SwipeDirection direction)
        {
            var swipe = new SwipeGestureRecognizer { Direction = direction };
            swipe.Swiped += async (sender, e) =>
            {
                await AnimateSwipeDirection(direction);
                Swipe?.Invoke(this, e);
            };
            return swipe;
        }

        private async Task AnimateSwipeDirection(SwipeDirection direction)
        {
            var originalColor = containerFrame.BackgroundColor;
            var animationColor = GetDirectionColor(direction);

            // Quick color flash animation
            await containerFrame.FadeToAsync(0.7, 100);
            containerFrame.BackgroundColor = animationColor;
            await containerFrame.FadeToAsync(1, 100);
            
            // Scale animation
            await containerFrame.ScaleToAsync(1.1, 100);
            await containerFrame.ScaleToAsync(1, 100);
            
            // Restore original color
            containerFrame.BackgroundColor = originalColor;
        }

        private Color GetDirectionColor(SwipeDirection direction)
        {
            return direction switch
            {
                SwipeDirection.Up => Color.FromArgb("#4CAF50"),    // Green
                SwipeDirection.Down => Color.FromArgb("#f44336"),  // Red
                SwipeDirection.Left => Color.FromArgb("#2196F3"),  // Blue
                SwipeDirection.Right => Color.FromArgb("#FF9800"), // Orange
                _ => Colors.Gray
            };
        }
    }
}

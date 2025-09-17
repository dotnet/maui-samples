using Microsoft.Maui.Controls.Shapes;

namespace DragAndDropGesture
{
    public partial class CustomDataPackageDemoPage : ContentPage
    {
        const double area = 200 * 200;

        public CustomDataPackageDemoPage()
        {
            InitializeComponent();
        }

        void OnDragStarting(object sender, DragStartingEventArgs e)
        {
            Shape shape = (sender as Element)?.Parent as Shape;
            if (shape != null)
            {
                e.Data.Properties.Add("Square", new Square(shape.Width, shape.Height));
            }
        }

        async void OnDrop(object sender, DropEventArgs e)
        {
            if (e.Data.Properties.TryGetValue("Square", out var squareObj) && squareObj is Square square)
            {
                if (square.Area.Equals(area))
                {
                    await DisplayAlert("Correct! 🎉", "Congratulations! You found the square with the largest area!", "OK");
                }
                else
                {
                    await DisplayAlert("Try Again 🤔", $"This square has an area of {square.Area:F0}. Look for the one with area {area:F0}!", "OK");
                }
            }
        }
    }
}

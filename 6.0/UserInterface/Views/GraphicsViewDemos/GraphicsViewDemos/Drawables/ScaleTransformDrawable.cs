namespace GraphicsViewDemos.Drawables
{
    internal class ScaleTransformDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 4;
            canvas.StrokeDashPattern = new float[] { 2, 2 };
            canvas.FontColor = Colors.Blue;
            canvas.FontSize = 18;

            canvas.DrawRoundedRectangle(50, 50, 80, 20, 5);
            canvas.DrawString(".NET MAUI", 50, 50, 80, 20, HorizontalAlignment.Left, VerticalAlignment.Top);

            canvas.Scale(2, 2);
            canvas.DrawRoundedRectangle(50, 100, 80, 20, 5);
            canvas.DrawString(".NET MAUI", 50, 100, 80, 20, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
    }
}

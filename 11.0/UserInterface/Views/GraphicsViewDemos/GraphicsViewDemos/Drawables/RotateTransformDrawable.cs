namespace GraphicsViewDemos.Drawables
{
    internal class RotateTransformDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FontColor = App.SecondaryColor;
            canvas.FontSize = 18;

            canvas.Rotate(45);
            canvas.DrawString(".NET MAUI", 50, 50, HorizontalAlignment.Left);

            //canvas.Rotate(45, dirtyRect.Center.X, dirtyRect.Center.Y);
            //canvas.DrawString(".NET MAUI", dirtyRect.Center.X, dirtyRect.Center.Y, HorizontalAlignment.Left);
        }
    }
}

namespace GraphicsViewDemos.Drawables
{
    internal class EvenOddWindingModeDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float radius = 0.45f * Math.Min(dirtyRect.Width, dirtyRect.Height);

            PathF path = new PathF();
            path.MoveTo(dirtyRect.Center.X, dirtyRect.Center.Y - radius);

            for (int i = 1; i < 5; i++)
            {
                double angle = i * 4 * Math.PI / 5;
                path.LineTo(new PointF(radius * (float)Math.Sin(angle) + dirtyRect.Center.X, -radius * (float)Math.Cos(angle) + dirtyRect.Center.Y));
            }
            path.Close();

            canvas.StrokeSize = 15;
            canvas.StrokeLineJoin = LineJoin.Round;
            canvas.StrokeColor = App.PrimaryColor;
            canvas.FillColor = App.SecondaryColor;
            canvas.FillPath(path, WindingMode.EvenOdd);
            canvas.DrawPath(path);
        }
    }
}

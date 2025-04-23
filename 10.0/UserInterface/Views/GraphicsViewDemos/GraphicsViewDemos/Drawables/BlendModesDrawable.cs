namespace GraphicsViewDemos.Drawables
{
    internal class BlendModesDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            PointF center = new PointF(dirtyRect.Center.X, dirtyRect.Center.Y);
            float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 4;
            float distance = 0.8f * radius;

            PointF center1 = new PointF(distance * (float)Math.Cos(9 * Math.PI / 6) + center.X,
                distance * (float)Math.Sin(9 * Math.PI / 6) + center.Y);
            PointF center2 = new PointF(distance * (float)Math.Cos(1 * Math.PI / 6) + center.X,
                distance * (float)Math.Sin(1 * Math.PI / 6) + center.Y);
            PointF center3 = new PointF(distance * (float)Math.Cos(5 * Math.PI / 6) + center.X,
                distance * (float)Math.Sin(5 * Math.PI / 6) + center.Y);

            canvas.BlendMode = BlendMode.Multiply;

            canvas.FillColor = Colors.Cyan;
            canvas.FillCircle(center1, radius);

            canvas.FillColor = Colors.Magenta;
            canvas.FillCircle(center2, radius);

            canvas.FillColor = Colors.Yellow;
            canvas.FillCircle(center3, radius);
        }
    }
}

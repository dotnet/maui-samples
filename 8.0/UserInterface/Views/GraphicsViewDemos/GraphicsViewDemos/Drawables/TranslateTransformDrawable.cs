namespace GraphicsViewDemos.Drawables
{
    internal class TranslateTransformDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            PathF path = new PathF();
            for (int i = 0; i < 11; i++)
            {
                double angle = 5 * i * 2 * Math.PI / 11;
                PointF point = new PointF(100 * (float)Math.Sin(angle), -100 * (float)Math.Cos(angle));

                if (i == 0)
                    path.MoveTo(point);
                else
                    path.LineTo(point);
            }

            canvas.FillColor = App.PrimaryColor;
            canvas.Translate(150, 150);
            canvas.FillPath(path);
        }
    }
}

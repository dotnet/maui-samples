using System.Numerics;

namespace GraphicsViewDemos.Drawables
{
    internal class AffineTransformDrawable : IDrawable
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

            Matrix3x2 transform = new Matrix3x2(1.5f, 1, 0, 1, 150, 150);
            canvas.ConcatenateTransform(transform);
            canvas.FillColor = App.SecondaryColor;
            canvas.FillPath(path);
        }
    }
}

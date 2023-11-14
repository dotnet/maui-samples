namespace GraphicsViewDemos.Drawables
{
    internal class HorizontalLinearGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            {
                StartColor = Colors.Yellow,
                EndColor = Colors.Green,
                // StartPoint is already (0,0)
                EndPoint = new Point(1, 0)
            };

            RectF linearRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(linearGradientPaint, linearRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(linearRectangle, 12);
        }
    }

    internal class VerticalLinearGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            {
                StartColor = Colors.Yellow,
                EndColor = Colors.Green,
                // StartPoint is already (0,0)
                EndPoint = new Point(0, 1)
            };

            RectF linearRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(linearGradientPaint, linearRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(linearRectangle, 12);
        }
    }

    internal class DiagonalLinearGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            {
                StartColor = Colors.Yellow,
                EndColor = Colors.Green,
                // StartPoint is already (0,0)
                // EndPoint is already (1,1)
            };

            RectF linearRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(linearGradientPaint, linearRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(linearRectangle, 12);
        }
    }

    internal class FourColorLinearGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            {
                StartColor = Colors.Yellow,
                EndColor = Colors.Green,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            linearGradientPaint.AddOffset(0.25f, App.PrimaryColor);
            linearGradientPaint.AddOffset(0.75f, App.SecondaryColor);

            RectF linearRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(linearGradientPaint, linearRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(linearRectangle, 12);
        }
    }

}

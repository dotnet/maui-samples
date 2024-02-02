namespace GraphicsViewDemos.Drawables
{
    internal class CenteredRadialGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            RadialGradientPaint radialGradientPaint = new RadialGradientPaint
            {
                StartColor = App.PrimaryColor,
                EndColor = Colors.DarkBlue
                // Center is already (0.5,0.5)
                // Radius is already 0.5
            };

            RectF radialRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(radialGradientPaint, radialRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(radialRectangle, 12);
        }
    }

    internal class TopLeftRadialGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            RadialGradientPaint radialGradientPaint = new RadialGradientPaint
            {
                StartColor = App.PrimaryColor,
                EndColor = Colors.DarkBlue,
                Center = new Point(0.0, 0.0)
                // Radius is already 0.5
            };

            RectF radialRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(radialGradientPaint, radialRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(radialRectangle, 12);
        }
    }

    internal class BottomRightRadialGradientPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            RadialGradientPaint radialGradientPaint = new RadialGradientPaint
            {
                StartColor = App.PrimaryColor,
                EndColor = Colors.DarkBlue,
                Center = new Point(1.0, 1.0)
                // Radius is already 0.5
            };

            RectF radialRectangle = new RectF(10, 10, 200, 100);
            canvas.SetFillPaint(radialGradientPaint, radialRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(radialRectangle, 12);
        }
    }
}

namespace GraphicsViewDemos.Drawables
{
    internal class PatternPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IPattern pattern;

            // Create a 10x10 template for the pattern
            using (PictureCanvas picture = new PictureCanvas(0, 0, 10, 10))
            {
                picture.StrokeColor = Colors.Silver;
                picture.DrawLine(0, 0, 10, 10);
                picture.DrawLine(0, 10, 10, 0);
                pattern = new PicturePattern(picture.Picture, 10, 10);
            }

            // Fill the rectangle with the 10x10 pattern
            PatternPaint patternPaint = new PatternPaint
            {
                Pattern = pattern
            };
            canvas.SetFillPaint(patternPaint, RectF.Zero);
            canvas.FillRectangle(10, 10, 250, 250);
        }
    }
}

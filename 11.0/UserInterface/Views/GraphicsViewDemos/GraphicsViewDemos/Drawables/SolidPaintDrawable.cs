namespace GraphicsViewDemos.Drawables
{
    internal class SolidPaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            SolidPaint solidPaint = new SolidPaint(App.PrimaryColor);

            //SolidPaint solidPaint = new SolidPaint
            //{
            //    Color = Colors.Silver
            //};

            RectF solidRectangle = new RectF(100, 100, 200, 100);
            canvas.SetFillPaint(solidPaint, solidRectangle);
            canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
            canvas.FillRoundedRectangle(solidRectangle, 12);
        }
    }
}

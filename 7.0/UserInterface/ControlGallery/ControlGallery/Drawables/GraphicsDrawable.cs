using Microsoft.Maui.Graphics;
using System;

namespace ControlGallery.Drawables
{
    public class GraphicsDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeLineCap = LineCap.Round;
            canvas.FillColor = Colors.Gray;

            // Translation and scaling
            canvas.Translate(dirtyRect.Center.X, dirtyRect.Center.Y);
            float scale = Math.Min(dirtyRect.Width / 200f, dirtyRect.Height / 200f);
            canvas.Scale(scale, scale);

            // Hour and minute marks
            for (int angle = 0; angle < 360; angle += 6)
            {
                canvas.FillCircle(0, -90, angle % 30 == 0 ? 4 : 2);
                canvas.Rotate(6);
            }

            DateTime now = DateTime.Now;

            // Hour hand
            canvas.StrokeSize = 20;
            canvas.SaveState();
            canvas.Rotate(30 * now.Hour + now.Minute / 2f);
            canvas.DrawLine(0, 0, 0, -50);
            canvas.RestoreState();

            // Minute hand
            canvas.StrokeSize = 10;
            canvas.SaveState();
            canvas.Rotate(6 * now.Minute + now.Second / 10f);
            canvas.DrawLine(0, 0, 0, -70);
            canvas.RestoreState();

            // Second hand
            canvas.StrokeSize = 2;
            canvas.SaveState();
            canvas.Rotate(6 * now.Second);
            canvas.DrawLine(0, 10, 0, -80);
            canvas.RestoreState();
        }
    }
}
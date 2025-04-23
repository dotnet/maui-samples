using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos
{
    public class TouchPoint
    {
        // For painting
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Fill
        };

        // For dragging
        bool isBeingDragged;
        long touchId;
        SKPoint previousPoint;

        public SKPoint Center { get; set; }
        public float Radius { get; set; } = 75;
        public SKColor Color { get; set; } = new SKColor(0, 0, 255, 64);

        public TouchPoint()
        {
        }

        public TouchPoint(float x, float y)
        {
            Center = new SKPoint(x, y);
        }

        public void Paint(SKCanvas canvas)
        {
            paint.Color = Color;
            canvas.DrawCircle(Center.X, Center.Y, Radius, paint);
        }

        public bool ProcessTouchEvent(long id, SKTouchAction type, SKPoint location)
        {
            bool centerMoved = false;

            switch (type)
            {
                case SKTouchAction.Pressed:
                    if (!isBeingDragged && PointInCircle(location))
                    {
                        isBeingDragged = true;
                        touchId = id;
                        previousPoint = location;
                        centerMoved = false;
                    }
                    break;

                case SKTouchAction.Moved:
                    if (isBeingDragged && touchId == id)
                    {
                        Center += location - previousPoint;
                        previousPoint = location;
                        centerMoved = true;
                    }
                    break;

                case SKTouchAction.Released:
                    if (isBeingDragged && touchId == id)
                    {
                        Center += location - previousPoint;
                        isBeingDragged = false;
                        centerMoved = true;
                    }
                    break;

                case SKTouchAction.Cancelled:
                    isBeingDragged = false;
                    break;
            }
            return centerMoved;
        }

        bool PointInCircle(SKPoint pt)
        {
            return (Math.Pow(pt.X - Center.X, 2) + Math.Pow(pt.Y - Center.Y, 2)) < (Radius * Radius);
        }
    }
}


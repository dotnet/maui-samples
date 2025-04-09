using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos
{
    public class InteractivePage : ContentPage
    {
        protected SKCanvasView baseCanvasView;
        protected TouchPoint[] touchPoints;

        protected SKPaint strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 3
        };

        protected SKPaint redStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 15
        };

        protected SKPaint dottedStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 3,
            PathEffect = SKPathEffect.CreateDash(new float[] { 7, 7 }, 0)
        };

        protected void OnTouch(object sender, SKTouchEventArgs e)
        { 
            bool touchPointMoved = false;

            foreach (TouchPoint touchPoint in touchPoints)
            {
                float scale = baseCanvasView.CanvasSize.Width / (float)baseCanvasView.Width;
                SKPoint point = new SKPoint(scale * (float)e.Location.X,
                                            scale * (float)e.Location.Y);
                touchPointMoved |= touchPoint.ProcessTouchEvent(e.Id, e.ActionType, point);
            }

            if (touchPointMoved)
            {
                baseCanvasView.InvalidateSurface();
            }

            e.Handled = true;
        }
    }
}


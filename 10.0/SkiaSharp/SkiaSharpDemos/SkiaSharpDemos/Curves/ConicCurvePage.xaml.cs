﻿using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos.Curves;

public partial class ConicCurvePage : InteractivePage
{
    public ConicCurvePage()
    {
        touchPoints = new TouchPoint[3];

        for (int i = 0; i < 3; i++)
        {
            TouchPoint touchPoint = new TouchPoint
            {
                Center = new SKPoint(100 + 100 * i,
                                     100 + (i == 1 ? 300 : 0))
            };
            touchPoints[i] = touchPoint;
        }

        InitializeComponent();
        baseCanvasView = canvasView;
        weightSlider.Value = 0.5;
    }

    void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
    {
        canvasView.InvalidateSurface();
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        // Draw path with conic curve
        using (SKPath path = new SKPath())
        {
            path.MoveTo(touchPoints[0].Center);
            path.ConicTo(touchPoints[1].Center,
                         touchPoints[2].Center,
                         (float)weightSlider.Value);

            canvas.DrawPath(path, strokePaint);
        }

        // Draw tangent lines
        canvas.DrawLine(touchPoints[0].Center.X,
                        touchPoints[0].Center.Y,
                        touchPoints[1].Center.X,
                        touchPoints[1].Center.Y, dottedStrokePaint);

        canvas.DrawLine(touchPoints[1].Center.X,
                        touchPoints[1].Center.Y,
                        touchPoints[2].Center.X,
                        touchPoints[2].Center.Y, dottedStrokePaint);

        foreach (TouchPoint touchPoint in touchPoints)
        {
            touchPoint.Paint(canvas);
        }
    }
}
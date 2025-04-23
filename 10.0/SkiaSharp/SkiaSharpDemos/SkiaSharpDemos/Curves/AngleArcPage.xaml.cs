﻿using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos.Curves;

public partial class AngleArcPage : ContentPage
{
    SKPaint outlinePaint = new SKPaint
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 3,
        Color = SKColors.Black
    };

    SKPaint arcPaint = new SKPaint
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 15,
        Color = SKColors.Red
    };

    public AngleArcPage()
    {
        InitializeComponent();
    }

    void sliderValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (canvasView != null)
        {
            canvasView.InvalidateSurface();
        }
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        SKRect rect = new SKRect(100, 100, info.Width - 100, info.Height - 100);
        float startAngle = (float)startAngleSlider.Value;
        float sweepAngle = (float)sweepAngleSlider.Value;

        canvas.DrawOval(rect, outlinePaint);

        using (SKPath path = new SKPath())
        {
            path.AddArc(rect, startAngle, sweepAngle);
            canvas.DrawPath(path, arcPaint);
        }
    }
}
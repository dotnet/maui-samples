﻿using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos.Curves;

public partial class EllipticalArcPage : InteractivePage
{
    SKColor[] colors = { SKColors.Red, SKColors.Green, SKColors.Blue, SKColors.Gray };

    public EllipticalArcPage()
    {
        touchPoints = new TouchPoint[2];

        for (int i = 0; i < 2; i++)
        {
            TouchPoint touchPoint = new TouchPoint
            {
                Center = new SKPoint(200, 100 + 200 * i)
            };
            touchPoints[i] = touchPoint;
        }
        InitializeComponent();

        baseCanvasView = canvasView;
        xRadiusSlider.Value = 125;
        yRadiusSlider.Value = 120;
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

        using (SKPath path = new SKPath())
        {
            int colorIndex = 0;
            SKPoint ellipseSize = new SKPoint((float)xRadiusSlider.Value,
                                              (float)yRadiusSlider.Value);
            float rotation = (float)rotationSlider.Value;

            foreach (SKPathArcSize arcSize in Enum.GetValues(typeof(SKPathArcSize)))
                foreach (SKPathDirection direction in Enum.GetValues(typeof(SKPathDirection)))
                {
                    path.MoveTo(touchPoints[0].Center);
                    path.ArcTo(ellipseSize, rotation,
                               arcSize, direction,
                               touchPoints[1].Center);

                    strokePaint.Color = colors[colorIndex++];
                    canvas.DrawPath(path, strokePaint);
                    path.Reset();
                }
        }

        foreach (TouchPoint touchPoint in touchPoints)
        {
            touchPoint.Paint(canvas);
        }
    }
}

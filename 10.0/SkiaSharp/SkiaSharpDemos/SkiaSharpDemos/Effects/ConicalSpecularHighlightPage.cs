﻿using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class ConicalSpecularHighlightPage : ContentPage
    {
        public ConicalSpecularHighlightPage()
        {
            Title = "Conical Specular Highlight";

            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            float radius = 0.4f * Math.Min(info.Width, info.Height);
            SKPoint center = new SKPoint(info.Rect.MidX, info.Rect.MidY);
            SKPoint offCenter = center - new SKPoint(radius / 2, radius / 2);

            using (SKPaint paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateTwoPointConicalGradient(
                                    offCenter,
                                    1,
                                    center,
                                    radius,
                                    new SKColor[] { SKColors.White, SKColors.Red },
                                    null,
                                    SKShaderTileMode.Clamp);

                canvas.DrawCircle(center, radius, paint);
            }
        }
    }
}


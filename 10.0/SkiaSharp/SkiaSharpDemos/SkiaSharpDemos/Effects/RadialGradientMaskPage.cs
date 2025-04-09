﻿using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class RadialGradientMaskPage : ContentPage
    {
        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(
           typeof(RadialGradientMaskPage),
           "SkiaSharpDemos.Media.mountainclimbers.jpg");

        static readonly SKPoint CENTER = new SKPoint(180, 300);
        static readonly float RADIUS = 120;

        public RadialGradientMaskPage()
        {
            Title = "Radial Gradient Mask";

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

            // Find rectangle to display bitmap
            float scale = Math.Min((float)info.Width / bitmap.Width,
                                   (float)info.Height / bitmap.Height);

            SKRect rect = SKRect.Create(scale * bitmap.Width, scale * bitmap.Height);

            float x = (info.Width - rect.Width) / 2;
            float y = (info.Height - rect.Height) / 2;
            rect.Offset(x, y);

            // Display bitmap in rectangle
            canvas.DrawBitmap(bitmap, rect);

            // Adjust center and radius for scaled and offset bitmap
            SKPoint center = new SKPoint(scale * CENTER.X + x,
                                         scale * CENTER.Y + y);
            float radius = scale * RADIUS;

            using (SKPaint paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateRadialGradient(
                                    center,
                                    radius,
                                    new SKColor[] { SKColors.Transparent,
                                                    SKColors.White },
                                    new float[] { 0.6f, 1 },
                                    SKShaderTileMode.Clamp);

                // Display rectangle using that gradient
                canvas.DrawRect(rect, paint);
            }
        }
    }
}


﻿using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class PastelMatrixPage : ContentPage
    {
        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(
                            typeof(PastelMatrixPage),
                            "SkiaSharpDemos.Media.mountainclimbers.jpg");

        public PastelMatrixPage()
        {
            Title = "Pastel Matrix";

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

            using (SKPaint paint = new SKPaint())
            {
                paint.ColorFilter =
                    SKColorFilter.CreateColorMatrix(new float[]
                    {
                        0.75f, 0.25f, 0.25f, 0, 0,
                        0.25f, 0.75f, 0.25f, 0, 0,
                        0.25f, 0.25f, 0.75f, 0, 0,
                        0, 0, 0, 1, 0
                    });

                canvas.DrawBitmap(bitmap, info.Rect, BitmapStretch.Uniform, paint: paint);
            }
        }
    }
}


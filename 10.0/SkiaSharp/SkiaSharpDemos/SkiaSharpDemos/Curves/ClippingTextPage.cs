﻿using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Curves
{
    public class ClippingTextPage : ContentPage
    {
        SKBitmap bitmap;

        public ClippingTextPage()
        {
            Title = "Clipping Text";

            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;

            string resourceID = "SkiaSharpDemos.Media.pageofcode.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                bitmap = SKBitmap.Decode(stream);
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.Blue);

            using (SKPaint paint = new SKPaint())
            {
                paint.Typeface = SKTypeface.FromFamilyName(null, SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                paint.TextSize = 10;

                using (SKPath textPath = paint.GetTextPath("CODE", 0, 0))
                {
                    // Set transform to center and enlarge clip path to window height
                    SKRect bounds;
                    textPath.GetTightBounds(out bounds);

                    canvas.Translate(info.Width / 2, info.Height / 2);
                    canvas.Scale(info.Width / bounds.Width, info.Height / bounds.Height);
                    canvas.Translate(-bounds.MidX, -bounds.MidY);

                    // Set the clip path
                    canvas.ClipPath(textPath);
                }
            }

            // Reset transforms
            canvas.ResetMatrix();

            // Display bitmap to fill window but maintain aspect ratio
            SKRect rect = new SKRect(0, 0, info.Width, info.Height);
            canvas.DrawBitmap(bitmap,
                rect.AspectFill(new SKSize(bitmap.Width, bitmap.Height)));
        }
    }
}


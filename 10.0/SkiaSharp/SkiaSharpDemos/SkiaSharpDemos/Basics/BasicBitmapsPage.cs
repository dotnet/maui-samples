﻿using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Basics
{
    public class BasicBitmapsPage : ContentPage
    {
        SKCanvasView canvasView;

        HttpClient httpClient = new HttpClient();

        SKBitmap webBitmap;
        SKBitmap resourceBitmap;
        SKBitmap libraryBitmap;

        public BasicBitmapsPage()
        {
            Title = "Basic Bitmaps";

            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;

            // Load resource bitmap
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream("SkiaSharpDemos.Media.monkey.png"))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            // Add tap gesture recognizer
            TapGestureRecognizer tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += async (sender, args) =>
            {
                // Load bitmap from photo library
                FileResult photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    using (Stream stream = await photo.OpenReadAsync())
                    {
                        if (stream != null)
                        {
                            libraryBitmap = SKBitmap.Decode(stream);
                            canvasView.InvalidateSurface();
                        }
                    }
                }
            };
            canvasView.GestureRecognizers.Add(tapRecognizer);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load web bitmap.
            string url = "https://learn.microsoft.com/en-us/dotnet/maui/media/what-is-maui/maui-overview.png";

            try
            {
                using (Stream stream = await httpClient.GetStreamAsync(url))
                using (MemoryStream memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);

                    webBitmap = SKBitmap.Decode(memStream);
                    canvasView.InvalidateSurface();
                }
            }
            catch
            {
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (webBitmap != null)
            {
                float x = (info.Width - webBitmap.Width) / 2;
                float y = (info.Height / 3 - webBitmap.Height) / 2;
                canvas.DrawBitmap(webBitmap, x, y);
            }

            if (resourceBitmap != null)
            {
                canvas.DrawBitmap(resourceBitmap,
                    new SKRect(0, info.Height / 3, info.Width, 2 * info.Height / 3));
            }

            if (libraryBitmap != null)
            {
                float scale = Math.Min((float)info.Width / libraryBitmap.Width,
                                       info.Height / 3f / libraryBitmap.Height);

                float left = (info.Width - scale * libraryBitmap.Width) / 2;
                float top = (info.Height / 3 - scale * libraryBitmap.Height) / 2;
                float right = left + scale * libraryBitmap.Width;
                float bottom = top + scale * libraryBitmap.Height;
                SKRect rect = new SKRect(left, top, right, bottom);
                rect.Offset(0, 2 * info.Height / 3);

                canvas.DrawBitmap(libraryBitmap, rect);
            }
            else
            {
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = SKColors.Blue;
                    paint.TextAlign = SKTextAlign.Center;
                    paint.TextSize = 48;

                    canvas.DrawText("Tap to load bitmap", info.Width / 2, 5 * info.Height / 6, paint);
                }
            }
        }
    }
}


using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Bitmaps
{
    public class PixelDimensionsPage : ContentPage
    {
        SKBitmap bitmap;

        public PixelDimensionsPage()
        {
            Title = "Pixel Dimensions";

            // Load the bitmap from a resource
            string resourceID = "SkiaSharpDemos.Media.banana.jpg";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                bitmap = SKBitmap.Decode(stream);
            }

            // Create the SKCanvasView and set the PaintSurface handler
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

            float x = (info.Width - bitmap.Width) / 2;
            float y = (info.Height - bitmap.Height) / 2;

            canvas.DrawBitmap(bitmap, x, y);
        }
    }
}


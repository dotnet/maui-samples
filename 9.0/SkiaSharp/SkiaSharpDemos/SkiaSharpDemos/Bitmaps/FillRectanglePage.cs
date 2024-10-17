using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Bitmaps
{
    public class FillRectanglePage : ContentPage
    {
        SKBitmap bitmap =
            BitmapExtensions.LoadBitmapResource(typeof(FillRectanglePage),
                                                "SkiaSharpDemos.Media.banana.jpg");
        public FillRectanglePage()
        {
            Title = "Fill Rectangle";

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

            canvas.DrawBitmap(bitmap, info.Rect);
        }
    }
}


using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class GrayScaleMatrixPage : ContentPage
    {
        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(
                            typeof(GrayScaleMatrixPage),
                            "SkiaSharpDemos.Media.banana.jpg");

        public GrayScaleMatrixPage()
        {
            Title = "Gray-Scale Matrix";

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
                        0.21f, 0.72f, 0.07f, 0, 0,
                        0.21f, 0.72f, 0.07f, 0, 0,
                        0.21f, 0.72f, 0.07f, 0, 0,
                        0,     0,     0,     1, 0
                    });

                canvas.DrawBitmap(bitmap, info.Rect, BitmapStretch.Uniform, paint: paint);
            }
        }
    }
}


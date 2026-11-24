using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class StoneWallPage : ContentPage
    {
        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(
                            typeof(StoneWallPage),
                            "SkiaSharpDemos.Media.stonewalltile.jpg");

        public StoneWallPage()
        {
            Title = "Stone Wall";

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
                // Create scale transform
                SKMatrix matrix = SKMatrix.CreateScale(0.5f, 0.5f);

                // Create bitmap tiling
                paint.Shader = SKShader.CreateBitmap(bitmap,
                                                     SKShaderTileMode.Mirror,
                                                     SKShaderTileMode.Mirror,
                                                     matrix);
                // Draw background
                canvas.DrawRect(info.Rect, paint);
            }
        }
    }
}


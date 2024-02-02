using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class PhotographicBrickWallPage : ContentPage
    {
        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(
                            typeof(PhotographicBrickWallPage),
                            "SkiaSharpDemos.Media.brickwalltile.jpg");

        public PhotographicBrickWallPage()
        {
            Title = "Photographic Brick Wall";

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
                // Create bitmap tiling
                paint.Shader = SKShader.CreateBitmap(bitmap,
                                                     SKShaderTileMode.Mirror,
                                                     SKShaderTileMode.Mirror);
                // Draw background
                canvas.DrawRect(info.Rect, paint);
            }
        }
    }
}


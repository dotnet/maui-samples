using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Effects
{
    public class CenteredTilesPage : ContentPage
    {
        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(
                            typeof(CenteredTilesPage),
                            "SkiaSharpDemos.Media.monkey.png");

        public CenteredTilesPage()
        {
            Title = "Centered Tiles";

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

            // Find coordinates to center bitmap in canvas...
            float x = (info.Width - bitmap.Width) / 2f;
            float y = (info.Height - bitmap.Height) / 2f;

            using (SKPaint paint = new SKPaint())
            {
                // ... but use them to create a translate transform
                SKMatrix matrix = SKMatrix.CreateTranslation(x, y);
                paint.Shader = SKShader.CreateBitmap(bitmap,
                                                     SKShaderTileMode.Repeat,
                                                     SKShaderTileMode.Repeat,
                                                     matrix);

                // Use that tiled bitmap pattern to fill a circle
                canvas.DrawCircle(info.Rect.MidX, info.Rect.MidY,
                                  Math.Min(info.Width, info.Height) / 2,
                                  paint);
            }
        }
    }
}


using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaSharpDemos.Bitmaps
{
    public class LatticeDisplayPage : ContentPage
    {
        SKBitmap bitmap = NinePatchDisplayPage.FiveByFiveBitmap;

        public LatticeDisplayPage()
        {
            Title = "Lattice Display";

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

            SKLattice lattice = new SKLattice();
            lattice.XDivs = new int[] { 100, 200, 400 };
            lattice.YDivs = new int[] { 100, 300, 400 };

            int count = (lattice.XDivs.Length + 1) * (lattice.YDivs.Length + 1);
            //lattice.Flags = new SKLatticeFlags[count];

            canvas.DrawBitmapLattice(bitmap, lattice, info.Rect);
        }
    }
}


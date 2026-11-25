using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos.Transforms;

public partial class TouchManipulationPage : ContentPage
{
    TouchManipulationBitmap bitmap;
    List<long> touchIds = new List<long>();
    MatrixDisplay matrixDisplay = new MatrixDisplay();

    public TouchManipulationPage()
    {
        InitializeComponent();

        Assembly assembly = GetType().GetTypeInfo().Assembly;

        using (Stream stream = assembly.GetManifestResourceStream("SkiaSharpDemos.Media.mountainclimbers.jpg"))
        {
            SKBitmap bitmap = SKBitmap.Decode(stream);
            this.bitmap = new TouchManipulationBitmap(bitmap);
            this.bitmap.TouchManager.Mode = TouchManipulationMode.ScaleRotate;
        }
    }

    void OnTouchModePickerSelectedIndexChanged(object sender, EventArgs args)
    {
        if (bitmap != null)
        {
            Picker picker = (Picker)sender;
            bitmap.TouchManager.Mode = (TouchManipulationMode)picker.SelectedItem;
        }
    }

    void OnTouch(object sender, SKTouchEventArgs e)
    {
        // Convert point to pixels
        SKPoint point = new SKPoint((float)(canvasView.CanvasSize.Width * e.Location.X / canvasView.Width), (float)(canvasView.CanvasSize.Height * e.Location.Y / canvasView.Height));

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                if (bitmap.HitTest(point))
                {
                    touchIds.Add(e.Id);
                    bitmap.ProcessTouchEvent(e.Id, e.ActionType, point);
                    break;
                }
                break;

            case SKTouchAction.Moved:
                if (touchIds.Contains(e.Id))
                {
                    bitmap.ProcessTouchEvent(e.Id, e.ActionType, point);
                    canvasView.InvalidateSurface();
                }
                break;

            case SKTouchAction.Released:
            case SKTouchAction.Cancelled:
                if (touchIds.Contains(e.Id))
                {
                    bitmap.ProcessTouchEvent(e.Id, e.ActionType, point);
                    touchIds.Remove(e.Id);
                    canvasView.InvalidateSurface();
                }
                break;
        }

        e.Handled = true;
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        // Display the bitmap
        bitmap.Paint(canvas);

        // Display the matrix in the lower-right corner
        SKSize matrixSize = matrixDisplay.Measure(bitmap.Matrix);

        matrixDisplay.Paint(canvas, bitmap.Matrix, new SKPoint(info.Width - matrixSize.Width, info.Height - matrixSize.Height));
    }
}
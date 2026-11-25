using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos.Transforms;

public partial class BitmapScatterViewPage : ContentPage
{
    List<TouchManipulationBitmap> bitmapCollection = new List<TouchManipulationBitmap>();

    Dictionary<long, TouchManipulationBitmap> bitmapDictionary = new Dictionary<long, TouchManipulationBitmap>();

    public BitmapScatterViewPage()
    {
        InitializeComponent();

        // Load in all the available bitmaps
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        string[] resourceIDs = assembly.GetManifestResourceNames();
        SKPoint position = new SKPoint();

        foreach (string resourceID in resourceIDs)
        {
            if (resourceID.EndsWith(".png") ||
                resourceID.EndsWith(".jpg"))
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceID))
                {
                    SKBitmap bitmap = SKBitmap.Decode(stream);
                    bitmapCollection.Add(new TouchManipulationBitmap(bitmap)
                    {
                        Matrix = SKMatrix.CreateTranslation(position.X, position.Y),
                    });
                    position.X += 100;
                    position.Y += 100;
                }
            }
        }
    }

    void OnTouch(object sender, SKTouchEventArgs e)
    {
        // Convert point to pixels
        SKPoint point = new SKPoint((float)(canvasView.CanvasSize.Width * e.Location.X / canvasView.Width), (float)(canvasView.CanvasSize.Height * e.Location.Y / canvasView.Height));

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                for (int i = bitmapCollection.Count - 1; i >= 0; i--)
                {
                    TouchManipulationBitmap bitmap = bitmapCollection[i];

                    if (bitmap.HitTest(point))
                    {
                        // Move bitmap to end of collection
                        bitmapCollection.Remove(bitmap);
                        bitmapCollection.Add(bitmap);

                        // Do the touch processing
                        bitmapDictionary.Add(e.Id, bitmap);
                        bitmap.ProcessTouchEvent(e.Id, e.ActionType, point);
                        canvasView.InvalidateSurface();
                        break;
                    }
                }
                break;

            case SKTouchAction.Moved:
                if (bitmapDictionary.ContainsKey(e.Id))
                {
                    TouchManipulationBitmap bitmap = bitmapDictionary[e.Id];
                    bitmap.ProcessTouchEvent(e.Id, e.ActionType, point);
                    canvasView.InvalidateSurface();
                }
                break;

            case SKTouchAction.Released:
            case SKTouchAction.Cancelled:
                if (bitmapDictionary.ContainsKey(e.Id))
                {
                    TouchManipulationBitmap bitmap = bitmapDictionary[e.Id];
                    bitmap.ProcessTouchEvent(e.Id, e.ActionType, point);
                    bitmapDictionary.Remove(e.Id);
                    canvasView.InvalidateSurface();
                }
                break;
        }

        e.Handled = true;
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKCanvas canvas = args.Surface.Canvas;
        canvas.Clear();

        foreach (TouchManipulationBitmap bitmap in bitmapCollection)
        {
            bitmap.Paint(canvas);
        }
    }
}

﻿using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaSharpDemos.Transforms;

public partial class ShowNonAffineMatrixPage : ContentPage
{
    SKMatrix matrix;
    SKBitmap bitmap;
    SKSize bitmapSize;

    TouchPoint[] touchPoints = new TouchPoint[4];

    MatrixDisplay matrixDisplay = new MatrixDisplay
    {
        PerspectiveFormat = "F5"
    };

    public ShowNonAffineMatrixPage()
    {
        InitializeComponent();

        Assembly assembly = GetType().GetTypeInfo().Assembly;
        using (Stream stream = assembly.GetManifestResourceStream("SkiaSharpDemos.Media.banana.jpg"))
        {
            bitmap = SKBitmap.Decode(stream);
        }

        touchPoints[0] = new TouchPoint(100, 100);                   // upper-left corner
        touchPoints[1] = new TouchPoint(bitmap.Width, 100);          // upper-right corner
        touchPoints[2] = new TouchPoint(100, bitmap.Height + 100);   // lower-left corner
        touchPoints[3] = new TouchPoint(bitmap.Width, bitmap.Height + 100);     // lower-right corner

        bitmapSize = new SKSize(bitmap.Width, bitmap.Height);
        matrix = ComputeMatrix(bitmapSize, touchPoints[0].Center, touchPoints[1].Center,
                                           touchPoints[2].Center, touchPoints[3].Center);
    }

    void OnTouch(object sender, SKTouchEventArgs e)
    {
        bool touchPointMoved = false;

        foreach (TouchPoint touchPoint in touchPoints)
        {
            float scale = canvasView.CanvasSize.Width / (float)canvasView.Width;
            SKPoint point = new SKPoint(scale * (float)e.Location.X,
                                        scale * (float)e.Location.Y);
            touchPointMoved |= touchPoint.ProcessTouchEvent(e.Id, e.ActionType, point);
        }

        if (touchPointMoved)
        {
            matrix = ComputeMatrix(bitmapSize, touchPoints[0].Center, touchPoints[1].Center,
                                               touchPoints[2].Center, touchPoints[3].Center);
            canvasView.InvalidateSurface();
        }

        e.Handled = true;
    }

    static SKMatrix ComputeMatrix(SKSize size, SKPoint ptUL, SKPoint ptUR, SKPoint ptLL, SKPoint ptLR)
    {
        // Scale transform
        SKMatrix S = SKMatrix.CreateScale(1 / size.Width, 1 / size.Height);

        // Affine transform
        SKMatrix A = new SKMatrix
        {
            ScaleX = ptUR.X - ptUL.X,
            SkewY = ptUR.Y - ptUL.Y,
            SkewX = ptLL.X - ptUL.X,
            ScaleY = ptLL.Y - ptUL.Y,
            TransX = ptUL.X,
            TransY = ptUL.Y,
            Persp2 = 1
        };

        // Non-Affine transform
        SKMatrix inverseA;
        A.TryInvert(out inverseA);
        SKPoint abPoint = inverseA.MapPoint(ptLR);
        float a = abPoint.X;
        float b = abPoint.Y;

        float scaleX = a / (a + b - 1);
        float scaleY = b / (a + b - 1);

        SKMatrix N = new SKMatrix
        {
            ScaleX = scaleX,
            ScaleY = scaleY,
            Persp0 = scaleX - 1,
            Persp1 = scaleY - 1,
            Persp2 = 1
        };

        // Multiply S * N * A
        SKMatrix result = SKMatrix.CreateIdentity();
        result = result.PostConcat(S);
        result = result.PostConcat(N);
        result = result.PostConcat(A);

        return result;
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        // Display the bitmap using the matrix
        canvas.Save();
        canvas.SetMatrix(matrix);
        canvas.DrawBitmap(bitmap, 0, 0);
        canvas.Restore();

        // Display the matrix in the lower-right corner
        SKSize matrixSize = matrixDisplay.Measure(matrix);

        matrixDisplay.Paint(canvas, matrix,
            new SKPoint(info.Width - matrixSize.Width,
                        info.Height - matrixSize.Height));

        // Display the touchpoints
        foreach (TouchPoint touchPoint in touchPoints)
        {
            touchPoint.Paint(canvas);
        }
    }
}
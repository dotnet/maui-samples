﻿using SkiaSharp;

namespace PhotoPuzzle
{
    static class BitmapExtensions
    {
        public static void DrawBitmap(this SKCanvas canvas, SKBitmap bitmap, SKRect dest,
                                      BitmapStretch stretch,
                                      BitmapAlignment horizontal = BitmapAlignment.Center,
                                      BitmapAlignment vertical = BitmapAlignment.Center,
                                      SKPaint paint = null)
        {
            if (stretch == BitmapStretch.Fill)
            {
                canvas.DrawBitmap(bitmap, dest, paint);
            }
            else
            {
                float scale = 1;

                switch (stretch)
                {
                    case BitmapStretch.None:
                        break;

                    case BitmapStretch.Uniform:
                        scale = Math.Min(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
                        break;

                    case BitmapStretch.UniformToFill:
                        scale = Math.Max(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
                        break;
                }

                SKRect display = CalculateDisplayRect(dest, scale * bitmap.Width, scale * bitmap.Height,
                                                      horizontal, vertical);

                canvas.DrawBitmap(bitmap, display, paint);
            }
        }

        static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight,
                                           BitmapAlignment horizontal, BitmapAlignment vertical)
        {
            float x = 0;
            float y = 0;

            switch (horizontal)
            {
                case BitmapAlignment.Center:
                    x = (dest.Width - bmpWidth) / 2;
                    break;

                case BitmapAlignment.Start:
                    break;

                case BitmapAlignment.End:
                    x = dest.Width - bmpWidth;
                    break;
            }

            switch (vertical)
            {
                case BitmapAlignment.Center:
                    y = (dest.Height - bmpHeight) / 2;
                    break;

                case BitmapAlignment.Start:
                    break;

                case BitmapAlignment.End:
                    y = dest.Height - bmpHeight;
                    break;
            }

            x += dest.Left;
            y += dest.Top;

            return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
        }
    }

    public enum BitmapStretch
    {
        None,
        Fill,
        Uniform,
        UniformToFill,
        AspectFit = Uniform,
        AspectFill = UniformToFill
    }

    public enum BitmapAlignment
    {
        Start,
        Center,
        End
    }
}


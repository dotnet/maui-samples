﻿using SkiaSharp;

namespace SkiaSharpDemos.Effects
{
    public class PorterDuffGridPage : ContentPage
    {
        public PorterDuffGridPage()
        {
            Title = "Porter-Duff Grid";

            SKBlendMode[] blendModes =
            {
                SKBlendMode.Src, SKBlendMode.Dst, SKBlendMode.SrcOver, SKBlendMode.DstOver,
                SKBlendMode.SrcIn, SKBlendMode.DstIn, SKBlendMode.SrcOut, SKBlendMode.DstOut,
                SKBlendMode.SrcATop, SKBlendMode.DstATop, SKBlendMode.Xor, SKBlendMode.Plus,
                SKBlendMode.Modulate, SKBlendMode.Clear
            };

            Grid grid = new Grid
            {
                Margin = new Thickness(5)
            };

            for (int row = 0; row < 4; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            for (int col = 0; col < 4; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int i = 0; i < blendModes.Length; i++)
            {
                SKBlendMode blendMode = blendModes[i];
                int row = 2 * (i / 4);
                int col = i % 4;

                Label label = new Label
                {
                    Text = blendMode.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center
                };
                Grid.SetRow(label, row);
                Grid.SetColumn(label, col);
                grid.Add(label);

                PorterDuffCanvasView canvasView = new PorterDuffCanvasView(blendMode);

                Grid.SetRow(canvasView, row + 1);
                Grid.SetColumn(canvasView, col);
                grid.Add(canvasView);
            }

            Content = grid;
        }
    }
}


using System;
using Microcharts;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace PointOfSale.Pages.Views
{
    internal class ChartView : SKCanvasView
    {
        public event EventHandler<SKPaintSurfaceEventArgs> ChartPainted;

        public static readonly BindableProperty ChartProperty = BindableProperty.Create(nameof(Chart), typeof(Chart), typeof(ChartView), null, propertyChanged: OnChartChanged);
        
        public Chart Chart
        {
            get { return (Chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }

        private InvalidatedWeakEventHandler<ChartView> handler;

        private Chart _chart;

        public ChartView()
        {
            BackgroundColor = Colors.Transparent;
            PaintSurface += OnPaintCanvas;
        }

        private static void OnChartChanged(BindableObject d, object oldValue, object value)
        {
            var view = d as ChartView;

            if (view._chart != null)
            {
                view.handler.Dispose();
                view.handler = null;
            }

            view._chart = value as Chart;
            view.InvalidateSurface();

            if (view._chart != null)
            {
                view.handler = view._chart.ObserveInvalidate(view, (v) => v.InvalidateSurface());
            }
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_chart != null)
            {
                _chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }

            ChartPainted?.Invoke(sender, e);
        }
    }
}


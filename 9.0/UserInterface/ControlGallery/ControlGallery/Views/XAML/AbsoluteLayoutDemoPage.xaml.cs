using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ControlGallery.Views.XAML
{
    public partial class AbsoluteLayoutDemoPage : ContentPage
    {
        Timer timer;

        public AbsoluteLayoutDemoPage()
        {
            InitializeComponent();
        }

        ~AbsoluteLayoutDemoPage() => timer.Dispose();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DateTime beginTime = DateTime.Now;

            timer = new Timer(new TimerCallback((s) =>
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    double seconds = (DateTime.Now - beginTime).TotalSeconds;
                    double offset = 1 - Math.Abs((seconds % 2) - 1);

                    AbsoluteLayout.SetLayoutBounds(text1,
                        new Rect(offset, offset,
                            AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(text2,
                        new Rect(1 - offset, offset,
                            AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                })),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1.0 / 30));
        }
    }
}
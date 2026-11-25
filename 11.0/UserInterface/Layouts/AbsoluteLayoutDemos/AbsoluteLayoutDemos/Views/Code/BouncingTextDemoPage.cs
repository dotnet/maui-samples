using Microsoft.Maui.Layouts;
using Microsoft.Maui.Dispatching;

namespace AbsoluteLayoutDemos.Views.Code
{
    public class BouncingTextDemoPage : ContentPage
    {
        const double period = 2000;
        readonly DateTime now = DateTime.Now;
        Label label1;
        Label label2;
        IDispatcherTimer timer;

        public BouncingTextDemoPage()
        {
            label1 = new Label { Text = "Bouncing text", FontSize = 20 };
            label2 = new Label { Text = "Bouncing text", FontSize = 20 };

            AbsoluteLayout.SetLayoutFlags(label1, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutFlags(label2, AbsoluteLayoutFlags.PositionProportional);

            Title = "Bouncing text demo";
            Content = new AbsoluteLayout
            {
                Children =
                {
                    label1,
                    label2
                }
            };
        }

        ~BouncingTextDemoPage() => timer.Tick -= OnTimerTick;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(15);
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            timer.Stop();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - now;
            double t = (elapsed.TotalMilliseconds % period) / period;
            t = 2 * (t < 0.5 ? t : 1 - t);

            AbsoluteLayout.SetLayoutBounds(label1, new Rect(t, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutBounds(label2, new Rect(0.5, 1 - t, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        }
    }
}

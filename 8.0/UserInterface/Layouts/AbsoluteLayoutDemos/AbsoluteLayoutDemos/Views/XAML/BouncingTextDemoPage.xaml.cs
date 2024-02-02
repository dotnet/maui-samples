using Microsoft.Maui.Dispatching;

namespace AbsoluteLayoutDemos.Views.XAML
{
    public partial class BouncingTextDemoPage : ContentPage
    {
        const double period = 2000;
        readonly DateTime now = DateTime.Now;
        IDispatcherTimer timer;

        public BouncingTextDemoPage()
        {
            InitializeComponent();
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
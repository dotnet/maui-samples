using Microsoft.Maui.Dispatching;

namespace AbsoluteLayoutDemos.Views.XAML
{
    public partial class SimpleOverlayDemoPage : ContentPage
    {
        IDispatcherTimer timer;

        public SimpleOverlayDemoPage()
        {
            InitializeComponent();
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
            // Show overlay with ProgressBar
            overlay.IsVisible = true;

            TimeSpan duration = TimeSpan.FromSeconds(5);
            DateTime now = DateTime.Now;

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += (sender, e) =>
            {
                double progress = (DateTime.Now - now).TotalMilliseconds / duration.TotalMilliseconds;
                progressBar.Progress = progress;
                bool continueTimer = progress < 1;

                if (!continueTimer)
                {
                    overlay.IsVisible = false;
                    timer.Stop();
                }
            };
            timer.Start();
        }
    }
}

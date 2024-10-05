namespace ShapesDemos.Views
{
    public class SpiralRunnerDemoPage : SpiralDemoPage
    {
        Timer timer;

        public SpiralRunnerDemoPage()
        {
            polyline.StrokeDashArray.Add(4);
            polyline.StrokeDashArray.Add(2);
            double total = polyline.StrokeDashArray[0] + polyline.StrokeDashArray[1];

            timer = new Timer(new TimerCallback((s) =>
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    double secs = DateTime.Now.TimeOfDay.TotalSeconds;
                    polyline.StrokeDashOffset = total * (secs % 1);
                })),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMilliseconds(15));
        }

        ~SpiralRunnerDemoPage() => timer.Dispose();
    }
}


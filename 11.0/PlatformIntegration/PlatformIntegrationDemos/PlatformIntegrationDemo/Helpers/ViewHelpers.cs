namespace PlatformIntegrationDemo.Helpers
{
    public static class ViewHelpers
    {
        public static Rect GetAbsoluteBounds(this Microsoft.Maui.Controls.View element)
        {
            Element looper = element;

            var absoluteX = element.X + element.Margin.Top;
            var absoluteY = element.Y + element.Margin.Left;

            while (looper.Parent != null)
            {
                looper = looper.Parent;
                if (looper is Microsoft.Maui.Controls.View v)
                {
                    absoluteX += v.X + v.Margin.Top;
                    absoluteY += v.Y + v.Margin.Left;
                }
            }

            return new Rect(absoluteX, absoluteY, element.Width, element.Height);
        }
    }
}


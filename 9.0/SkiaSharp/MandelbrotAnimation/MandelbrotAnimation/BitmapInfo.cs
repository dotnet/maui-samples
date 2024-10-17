namespace MandelbrotAnimation
{
    class BitmapInfo
    {
        public int PixelWidth { get; private set; }

        public int PixelHeight { get; private set; }

        public int[] IterationCounts { get; private set; }

        public BitmapInfo(int pixelWidth, int pixelHeight, int[] iterationCounts)
        {
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            IterationCounts = iterationCounts;
        }
    }
}


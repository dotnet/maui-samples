using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;
#if IOS || ANDROID || MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#endif

namespace GraphicsViewDemos.Drawables
{
    internal class ImagePaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
#if IOS || ANDROID || MACCATALYST
            IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
            {
                // PlatformImage isn't currently supported on Windows.
                image = PlatformImage.FromStream(stream);
            }

            //if (image != null)
            //{
            //    ImagePaint imagePaint = new ImagePaint
            //    {
            //        Image = image.Downsize(100)
            //    };
            //    canvas.SetFillPaint(imagePaint, RectF.Zero);
            //    canvas.FillRectangle(0, 0, 240, 300);
            //}

            if (image != null)
            {
                canvas.SetFillImage(image.Downsize(100));
                canvas.FillRectangle(0, 0, 240, 300);
            }
#endif
        }
    }
}

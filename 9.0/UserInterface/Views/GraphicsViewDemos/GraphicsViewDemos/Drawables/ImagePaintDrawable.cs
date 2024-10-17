using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;
using Microsoft.Maui.Graphics.Platform;

namespace GraphicsViewDemos.Drawables
{
    internal class ImagePaintDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
            {
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
        }
    }
}

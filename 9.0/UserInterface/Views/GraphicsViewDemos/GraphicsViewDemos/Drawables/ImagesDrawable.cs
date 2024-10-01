using Microsoft.Maui.Graphics.Platform;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace GraphicsViewDemos.Drawables
{
    internal class LoadImageDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
            {
                image = PlatformImage.FromStream(stream);
            }

            if (image != null)
            {
                canvas.DrawImage(image, 10, 10, image.Width, image.Height);
            }
        }
    }

    internal class ResizeImageDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            //IImage image;
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
            //{
            //    image = PlatformImage.FromStream(stream);
            //}

            //if (image != null)
            //{
            //    IImage newImage = image.Resize(100, 60, ResizeMode.Stretch, true);
            //    canvas.DrawImage(newImage, 10, 10, newImage.Width, newImage.Height);
            //}
        }
    }

    internal class DownsizeImageDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
            {
                image = PlatformImage.FromStream(stream);
            }

            if (image != null)
            {
                IImage newImage = image.Downsize(100, true);
                canvas.DrawImage(newImage, 10, 10, newImage.Width, newImage.Height);
            }
        }
    }

    internal class SaveImageDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
            {
                image = PlatformImage.FromStream(stream);
            }

            // Save image to a memory stream
            if (image != null)
            {
                IImage newImage = image.Downsize(150, true);
                using (MemoryStream memStream = new MemoryStream())
                {
                    newImage.Save(memStream);
                }
                canvas.DrawImage(newImage, 10, 10, newImage.Width, newImage.Height);
            }
        }
    }
}

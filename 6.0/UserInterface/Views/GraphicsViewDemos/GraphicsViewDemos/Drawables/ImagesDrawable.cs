#if IOS || ANDROID || MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#endif
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace GraphicsViewDemos.Drawables
{
    internal class LoadImageDrawable : IDrawable
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

            if (image != null)
            {
                canvas.DrawImage(image, 10, 10, image.Width, image.Height);
            }
#endif
        }
    }

    internal class ResizeImageDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
//#if IOS || ANDROID || MACCATALYST
//            IImage image;
//            Assembly assembly = GetType().GetTypeInfo().Assembly;
//            using (Stream stream = assembly.GetManifestResourceStream("GraphicsViewDemos.Resources.Images.dotnet_bot.png"))
//            {
//                // PlatformImage isn't currently supported on Windows.
//                image = PlatformImage.FromStream(stream);
//            }

//            if (image != null)
//            {
//                IImage newImage = image.Resize(100, 60, ResizeMode.Stretch, true);
//                canvas.DrawImage(newImage, 10, 10, newImage.Width, newImage.Height);
//            }
//#endif
        }
    }

    internal class DownsizeImageDrawable : IDrawable
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

            if (image != null)
            {
                IImage newImage = image.Downsize(100, true);
                canvas.DrawImage(newImage, 10, 10, newImage.Width, newImage.Height);
            }
#endif
        }
    }

    internal class SaveImageDrawable : IDrawable
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
#endif
        }
    }
}

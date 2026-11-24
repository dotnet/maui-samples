using SkiaSharp;
using System.Reflection;

namespace CatClock
{
    static class BitmapExtensions
    {
        public static SKBitmap LoadBitmapResource(Type type, string resourceId)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            using (Stream? stream = assembly.GetManifestResourceStream(resourceId))
            {
                return SKBitmap.Decode(stream);
            }
        }
    }
}

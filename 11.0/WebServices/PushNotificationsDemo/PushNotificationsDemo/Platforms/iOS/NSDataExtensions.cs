using Foundation;
using System.Text;

namespace PushNotificationsDemo.Platforms.iOS;

internal static class NSDataExtensions
{
    internal static string ToHexString(this NSData data)
    {
        var bytes = data.ToArray();

        if (bytes == null)
            return null;

        StringBuilder sb = new StringBuilder(bytes.Length * 2);

        foreach (byte b in bytes)
            sb.AppendFormat("{0:x2}", b);

        return sb.ToString().ToUpperInvariant();
    }
}


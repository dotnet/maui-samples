using System.ComponentModel;

namespace VideoDemos.Controls
{
    public class VideoSourceConverter : TypeConverter, IExtendedTypeConverter
    {
        object IExtendedTypeConverter.ConvertFromInvariantString(string value, IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Uri uri;
                return Uri.TryCreate(value, UriKind.Absolute, out uri) && uri.Scheme != "file" ?
                    VideoSource.FromUri(value) : VideoSource.FromResource(value);
            }
            throw new InvalidOperationException("Cannot convert null or whitespace to VideoSource.");
        }
    }
}

using System.Globalization;

namespace ChatMobile.Client.Converters;

public class BoolToStatusIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool hasCredentials)
        {
            return hasCredentials ? "✅" : "❌";
        }
        return "❓";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
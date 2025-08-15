using System.Globalization;

namespace ChatMobile.Client.Converters;

public class BoolToCredentialStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool hasCredentials)
        {
            return hasCredentials ? "Credentials Configured" : "No Credentials";
        }
        return "Status Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
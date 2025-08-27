using System.Globalization;
using Microsoft.Maui.Controls;

namespace ChatVoice.Client.Converters;

public class BoolToCredentialStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var has = value is bool b && b;
        return has ? "Credentials loaded" : "Credentials not configured";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

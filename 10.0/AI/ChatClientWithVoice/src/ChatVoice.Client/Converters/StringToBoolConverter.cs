using System.Globalization;
using Microsoft.Maui.Controls;

namespace ChatVoice.Client.Converters;

public class StringToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s && !string.IsNullOrWhiteSpace(s);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

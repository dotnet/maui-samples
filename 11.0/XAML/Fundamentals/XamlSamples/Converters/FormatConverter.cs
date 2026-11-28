using System.Globalization;

namespace XamlSamples;

// Formats a value with the format string in ConverterParameter, such as "C" or "D",
// in the culture that the binding passes to the converter.
public class FormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        string.Format(culture, "{0:" + parameter + "}", value);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

using System.Globalization;

namespace WeatherTwentyOne.Converters;

public class MaxTempOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        const double max = 90 * 3;

        var maxTemp = System.Convert.ToDouble(value) * 3;
        var topMargin = max - maxTemp;

        return new Thickness(0, topMargin, 0, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
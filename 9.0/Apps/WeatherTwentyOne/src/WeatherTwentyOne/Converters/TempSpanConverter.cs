using System.Globalization;

namespace WeatherTwentyOne.Converters;

public class TempSpanConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // two values
        var minTemp = double.Parse(values[0].ToString()) * 3;
        var maxTemp = double.Parse(values[1].ToString()) * 3;

        var diff = maxTemp - minTemp;

        return diff;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

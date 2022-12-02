using System.Globalization;

namespace WeatherTwentyOne.Converters;

public class CheckedColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var target = (bool)value;
        //var allParams = ((string)parameter).Split((';')); // 0=normal, 1=selected

        if (target)
            return Colors.White;
        else
            return Colors.Transparent;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value;
    }
}
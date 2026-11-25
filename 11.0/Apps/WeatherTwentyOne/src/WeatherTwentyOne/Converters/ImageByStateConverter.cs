using System.Globalization;

namespace WeatherTwentyOne.Converters;

public class ImageByStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var target = (FlyoutItem)value;
        var allParams = ((string)parameter).Split((';')); // 0=normal, 1=selected

        if (target.IsChecked && allParams.Length > 1)
            return allParams[1];
        else
            return allParams[0];
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value;
    }
}
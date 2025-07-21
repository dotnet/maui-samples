using System.Globalization;

namespace TipCalc;

public class DoubleRoundingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType,
                          object? parameter, CultureInfo culture)
    {
        return Round(value is double d ? d : 0, parameter);
    }

    public object? ConvertBack(object? value, Type targetType,
                              object? parameter, CultureInfo culture)
    {
        return Round(value is double d ? d : 0, parameter);
    }

    double Round(double number, object? parameter)
    {
        double precision = 1;

        // Assume parameter is string encoding precision.
        if (parameter is string s && double.TryParse(s, out var parsed))
        {
            precision = parsed;
        }
        return precision * Math.Round(number / precision);
    }
}

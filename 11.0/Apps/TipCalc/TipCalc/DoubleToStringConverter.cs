using System.Globalization;

namespace TipCalc;

public class DoubleToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, 
                          object? parameter, CultureInfo culture)
    {
        // Assumes value is double.
        double number = value is double d ? d : 0;

        // Return empty string for a zero (good for Entry views).
        if (number == 0)
        {
            return "";
        }

        return number.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, 
                              object? parameter, CultureInfo culture)
    {
        double number = 0;
        Double.TryParse(value as string, out number);
        return number;
    }
}

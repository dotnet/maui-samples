using System.Globalization;

namespace Weather;

public class LongToDateTimeConverter : IValueConverter
{
    DateTime _time = new(1970, 1, 1, 0, 0, 0, 0);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long dateTime)
            return $"{_time.AddSeconds(dateTime).ToString()} UTC";
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

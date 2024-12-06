using System.Globalization;

namespace DataBindingDemos.Converters
{
    internal class NamedColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                NamedColor namedColor = (NamedColor)value;
                return namedColor.Color;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Globalization;

namespace DataBindingDemos
{
    public class ColorObjectConverter :  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NamedColor namedColor = (NamedColor)value;
            return (string)namedColor.FriendlyName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

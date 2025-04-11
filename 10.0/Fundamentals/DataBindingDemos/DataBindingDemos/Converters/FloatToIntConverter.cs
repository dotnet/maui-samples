using System.Globalization;

namespace DataBindingDemos
{
    public class FloatToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)Math.Round((float)value * GetParameter(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value / GetParameter(parameter);
        }

        double GetParameter(object parameter)
        {
            if (parameter is float)
                return (float)parameter;

            else if (parameter is int)
                return (int)parameter;

            else if (parameter is string)
                return float.Parse((string)parameter);

            return 1;
        }
    }
}

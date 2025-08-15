using System.Globalization;

namespace BillingService.Converters;

public class BoolToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramString)
        {
            var options = paramString.Split('|');
            if (options.Length == 2)
            {
                return boolValue ? options[0] : options[1];
            }
        }
        return "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramString)
        {
            var options = paramString.Split('|');
            if (options.Length == 2)
            {
                var colorString = boolValue ? options[0] : options[1];

                // Handle static resource references
                if (colorString.StartsWith("{StaticResource ") && colorString.EndsWith("}"))
                {
                    var resourceKey = colorString.Substring(16, colorString.Length - 17);
                    if (Application.Current?.Resources.TryGetValue(resourceKey, out var resource) == true)
                    {
                        if (resource is Color color)
                            return color;
                        if (resource is SolidColorBrush brush)
                            return brush.Color;
                    }
                    // Fallback for common resource keys
                    return resourceKey switch
                    {
                        "Primary" => Color.FromArgb("#512BD4"),
                        "Secondary" => Color.FromArgb("#DFD8F7"),
                        _ => Colors.Gray
                    };
                }

                // Handle direct color names and hex values
                try
                {
                    if (colorString.Equals("LightGray", StringComparison.OrdinalIgnoreCase))
                        return Colors.LightGray;
                    
                    return Color.FromArgb(colorString);
                }
                catch
                {
                    return Colors.Gray;
                }
            }
        }
        return Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }
}

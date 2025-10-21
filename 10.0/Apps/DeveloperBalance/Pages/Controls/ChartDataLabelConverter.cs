using System;
using System.Globalization;
using DeveloperBalance.Models;

namespace DeveloperBalance.Pages.Controls;

public class ChartDataLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CategoryChartData categoryData && parameter is string parameterValue)
        {
            return parameterValue?.ToLower() switch
            {
                "title" => categoryData.Title,
                "count" => categoryData.Count.ToString(),
                _ => value?.ToString()
            };
        }
        
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

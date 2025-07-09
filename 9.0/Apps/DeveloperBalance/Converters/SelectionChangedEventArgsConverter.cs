using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace DeveloperBalance.Converters;

public class SelectionChangedEventArgsConverter: BaseConverterOneWay<SelectionChangedEventArgs?, object?>
{
    public override object? ConvertFrom(SelectionChangedEventArgs? value, CultureInfo? culture)
    {
        return value;
    }

    public override object? DefaultConvertReturnValue { get; set; } = null;
}
using Microsoft.Maui.Platform;

namespace PickerDemo.Control;

public class CustomPicker : Picker, ICustomPicker
{
    public static readonly BindableProperty DialogBackgroundColorProperty =
  BindableProperty.Create(nameof(DialogBackgroundColor), typeof(Color), typeof(CustomPicker), null);

    public static readonly BindableProperty DialogTextColorProperty =
        BindableProperty.Create(nameof(DialogTextColor), typeof(Color), typeof(CustomPicker), null);

    public static readonly BindableProperty SelectedTextColorProperty =
            BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(CustomPicker), null);

    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    public Color DialogBackgroundColor
    {
        get => (Color)GetValue(DialogBackgroundColorProperty);
        set => SetValue(DialogBackgroundColorProperty, value);
    }

    public Color DialogTextColor
    {
        get => (Color)GetValue(DialogTextColorProperty);
        set => SetValue(DialogTextColorProperty, value);
    }
}
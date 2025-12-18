using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using WinColor = Windows.UI.Color;
using PickerDemo.Control;
using MauiColor = Microsoft.Maui.Graphics.Color;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace PickerDemo.Handlers;

public partial class CustomPickerHandler : PickerHandler
{
    protected override ComboBox CreatePlatformView()
    {
        var comboBox = base.CreatePlatformView();
        if (VirtualView is CustomPicker customPicker)
        {
            UpdateComboBoxStyle(comboBox, customPicker);
        }
        return comboBox;
    }

    public static void MapDialogBackgroundColor(CustomPickerHandler handler, CustomPicker picker)
    {
        if (handler.PlatformView is ComboBox comboBox)
        {
            UpdateComboBoxStyle(comboBox, picker);
        }
    }

    public static void MapDialogTextColor(CustomPickerHandler handler, CustomPicker picker)
    {
        if (handler.PlatformView is ComboBox comboBox)
        {
            UpdateComboBoxStyle(comboBox, picker);
        }
    }

    public static void MapSelectedTextColor(CustomPickerHandler handler, CustomPicker picker)
    {
        if (handler.PlatformView is ComboBox comboBox)
        {
            UpdateComboBoxStyle(comboBox, picker);
        }
    }

    static void UpdateComboBoxStyle(ComboBox comboBox, CustomPicker picker)
    {
        // Apply same colors to dropdown items
        var itemStyle = new Microsoft.UI.Xaml.Style(typeof(ComboBoxItem));

        // Apply dialog background color if provided
        if (picker.DialogBackgroundColor is not null)
        {
            var dialogBackgroundColor = ConvertToWinColor(picker.DialogBackgroundColor);
            itemStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(ComboBoxItem.BackgroundProperty, new SolidColorBrush(dialogBackgroundColor)));
        }

        // Apply dialog text color if provided
        if (picker.DialogTextColor is not null)
        {
            var textColor = ConvertToWinColor(picker.DialogTextColor);
            itemStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(ComboBoxItem.ForegroundProperty, new SolidColorBrush(textColor)));
        }

        // Apply selected text color if provided
        if (picker.SelectedTextColor is not null)
        {
            var selectedTextColor = ConvertToWinColor(picker.SelectedTextColor);
            var selectedForegroundBrush = new SolidColorBrush(selectedTextColor);
            // Add custom resources that will be used for selected state
            comboBox.Resources["ComboBoxItemForegroundSelected"] = selectedForegroundBrush;
        }

        comboBox.Resources[typeof(ComboBoxItem)] = itemStyle;
    }

    static WinColor ConvertToWinColor(MauiColor color)
    {
        return WinColor.FromArgb(
   (byte)(color.Alpha * 255),
      (byte)(color.Red * 255),
            (byte)(color.Green * 255),
    (byte)(color.Blue * 255)
);
    }
}
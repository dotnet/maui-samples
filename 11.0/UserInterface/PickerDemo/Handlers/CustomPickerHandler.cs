using Microsoft.Maui.Handlers;
using PickerDemo.Control;
using Microsoft.Maui;

namespace PickerDemo.Handlers;

public partial class CustomPickerHandler : PickerHandler, IElementHandler
{
    public static IPropertyMapper<CustomPicker, CustomPickerHandler> CustomMapper { get; } =
        new PropertyMapper<CustomPicker, CustomPickerHandler>(PickerHandler.Mapper)
        {
#if IOS || WINDOWS
            [nameof(CustomPicker.DialogBackgroundColor)] = MapDialogBackgroundColor,
            [nameof(CustomPicker.DialogTextColor)] = MapDialogTextColor,
            [nameof(CustomPicker.SelectedTextColor)] = MapSelectedTextColor
#endif
        };

    public CustomPickerHandler() : base(CustomMapper)
    {
    }

    public CustomPickerHandler(IPropertyMapper? mapper = null)
        : base(mapper ?? CustomMapper)
    {
    }
}
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using PickerDemo.Control;
using UIKit;
using Foundation;

namespace PickerDemo.Handlers;

public partial class CustomPickerHandler : PickerHandler
{
#if MACCATALYST
    CustomPicker? _customPicker;
#endif

    protected override MauiPicker CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();

        if (VirtualView is not CustomPicker customPicker)
            return platformView;

#if IOS
        ConfigurePickerView(platformView, customPicker);
#elif MACCATALYST
        _customPicker = customPicker;
#endif

        return platformView;
    }

    /// <summary>
    /// Maps the DialogBackgroundColor property to the platform picker view.
    /// </summary>
    public static void MapDialogBackgroundColor(CustomPickerHandler handler, CustomPicker picker)
    {
        if (handler?.PlatformView is null || picker is null)
            return;

#if IOS
        if (handler.PlatformView.InputView is UIPickerView pickerView)
        {
            pickerView.BackgroundColor = picker.DialogBackgroundColor?.ToPlatform() ?? UIColor.SystemBackground;
        }
#endif
    }

    /// <summary>
    /// Maps the DialogTextColor property to the platform picker view.
    /// Triggers a reload to update the text colors.
    /// </summary>
    public static void MapDialogTextColor(CustomPickerHandler handler, CustomPicker picker)
    {
    }

    /// <summary>
    /// Maps the SelectedTextColor property to the platform picker view.
    /// Triggers a reload to update the selected text color.
    /// </summary>
    public static void MapSelectedTextColor(CustomPickerHandler handler, CustomPicker picker)
    {
        if (handler?.PlatformView is null || picker is null)
            return;

#if IOS
        if (handler.PlatformView.InputView is UIPickerView pickerView)
        {
            // Reload the picker view so that selected text color is reflected instantly while the picker is open
            pickerView.ReloadAllComponents();
        }
#endif
    }

#if IOS
    /// <summary>
    /// Configures the iOS picker view with custom delegate and styling.
    /// </summary>
    void ConfigurePickerView(MauiPicker platformView, CustomPicker customPicker)
    {
        if (platformView.InputView is not UIPickerView pickerView)
            return;

        pickerView.BackgroundColor = customPicker.DialogBackgroundColor?.ToPlatform() ?? UIColor.SystemBackground;

        // Set custom delegate to enable text color customization via GetAttributedTitle
        pickerView.Delegate = new CustomPickerViewDelegate(pickerView, customPicker);

        // Reload to apply the custom delegate and styling
        pickerView.ReloadAllComponents();
    }
#endif

#if MACCATALYST
    protected override void ConnectHandler(MauiPicker platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Started += OnPickerStarted;
    }

    protected override void DisconnectHandler(MauiPicker platformView)
    {
        platformView.Started -= OnPickerStarted;
        base.DisconnectHandler(platformView);
    }

    /// <summary>
    /// Handles the picker started event for Mac Catalyst.
    /// Schedules customization of the alert controller after a short delay.
    /// </summary>
    void OnPickerStarted(object? sender, EventArgs e)
    {
        if (_customPicker is null)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // Wait for the alert controller to be presented
            await Task.Delay(100);
            FindAndCustomizeAlertController(_customPicker);
        });
    }

    /// <summary>
    /// Finds and customizes the UIAlertController used by Mac Catalyst picker.
    /// </summary>
    void FindAndCustomizeAlertController(CustomPicker customPicker)
    {
        // Search through all window scenes to find the currently presented UIAlertController
        var alertController = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .SelectMany(scene => scene.Windows)
            .Select(w => w.RootViewController?.PresentedViewController)
            .OfType<UIAlertController>()
            .FirstOrDefault(ac => ac.View is not null);

        if (alertController?.View is null)
            return;

        // Apply background color to the alert view
        alertController.View.BackgroundColor = customPicker.DialogBackgroundColor?.ToPlatform() ?? UIColor.SystemBackground;

        // Find and customize the picker view within the alert
        var pickerView = alertController.View.Subviews.OfType<UIPickerView>().LastOrDefault();
        if (pickerView is not null)
        {
            pickerView.Delegate = new CustomPickerViewDelegate(pickerView, customPicker);
        }
    }
#endif
}

/// <summary>
/// Custom UIPickerViewDelegate that applies text colors based on selection state.
/// </summary>
public sealed class CustomPickerViewDelegate : UIPickerViewDelegate
{
    private readonly CustomPicker _customPicker;
    private readonly PickerSource? _originalModel;

    public CustomPickerViewDelegate(UIPickerView pickerView, CustomPicker customPicker)
    {
        _customPicker = customPicker ?? throw new ArgumentNullException(nameof(customPicker));
        if (pickerView.Model is not null)
        {
            // Store reference to the original PickerSource model to forward selection events
            _originalModel = pickerView.Model as PickerSource;
        }
    }

    public override NSAttributedString GetAttributedTitle(UIPickerView pickerView, nint row, nint component)
    {
        // Use the passed pickerView parameter for consistency across iOS and macOS
        var title = pickerView.Model?.GetTitle(pickerView, row, component) ?? string.Empty;
        var selectedRow = pickerView.SelectedRowInComponent(component);

        // Determine text color based on whether this row is selected
        var textColor = row == selectedRow
            ? _customPicker.SelectedTextColor?.ToPlatform() ?? UIColor.SystemBlue
            : _customPicker.DialogTextColor?.ToPlatform() ?? UIColor.Label;

        return new NSAttributedString(title, new UIStringAttributes
        {
            ForegroundColor = textColor,
        });
    }

    public override void Selected(UIPickerView pickerView, nint row, nint component)
    {
        // Forward the selection to the original PickerSource model
        _originalModel?.Selected(pickerView, row, component);

        // Reload components to update the text colors based on new selection
        pickerView.ReloadAllComponents();
    }
}
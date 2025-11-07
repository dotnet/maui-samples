using System.Reflection;
using Microsoft.Maui.Handlers;
using Android.Widget;
using AppCompatAlertDialog = AndroidX.AppCompat.App.AlertDialog;
using MauiPicker = Microsoft.Maui.Platform.MauiPicker;
using PickerDemo.Control;
using Microsoft.Maui.Platform;
using AndroidView = Android.Views.View;
using AndroidViewGroup = Android.Views.ViewGroup;

namespace PickerDemo.Handlers;

public partial class CustomPickerHandler : PickerHandler
{
    // TODO: Refactor to avoid reflection if MAUI framework exposes dialog customization in future.
    private FieldInfo? _dialogFieldInfo;

    protected override void ConnectHandler(MauiPicker platformView)
    {
        base.ConnectHandler(platformView);

        // Cache the FieldInfo (only done once)
        _dialogFieldInfo ??= GetDialogField();

        platformView.Click += OnCustomizeDialog;
    }

    // TODO: Refactor to avoid reflection if MAUI framework exposes dialog customization in future.
    static FieldInfo? GetDialogField()
    {
        return typeof(PickerHandler).GetField("_dialog",
            BindingFlags.NonPublic |
            BindingFlags.Instance);
    }

    protected override void DisconnectHandler(MauiPicker platformView)
    {
        platformView.Click -= OnCustomizeDialog;
        base.DisconnectHandler(platformView);
    }

    void OnCustomizeDialog(object? sender, EventArgs e)
    {
        if (VirtualView is not CustomPicker customPicker)
        {
            return;
        }

        // Get dialog via reflection (unavoidable without framework changes)
        var dialog = _dialogFieldInfo?.GetValue(this) as AppCompatAlertDialog;

        // Apply background color to the dialog window
        if (dialog?.Window is not null && customPicker.DialogBackgroundColor is not null)
        {
            dialog.Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(customPicker.DialogBackgroundColor.ToPlatform()));
        }

        var dialogListView = dialog?.ListView;
        if (dialogListView?.Adapter is not null)
        {
            // Wrap the original adapter with custom adapter
            var customAdapter = new ColoredPickerAdapter(
                dialogListView.Adapter,
                customPicker.DialogTextColor,
                customPicker.SelectedTextColor,
                VirtualView?.SelectedIndex ?? -1);

            // Set the custom adapter - this ensures GetView() is called for each item
            dialogListView.Adapter = customAdapter;
            if (VirtualView?.SelectedIndex >= 0)
            {
                dialogListView.SetItemChecked(VirtualView.SelectedIndex, true);
            }
        }
    }
}

/// <summary>
/// Custom adapter that wraps the original picker adapter and applies custom text colors.
/// This ensures colors are applied during view creation.
/// </summary>
internal class ColoredPickerAdapter : BaseAdapter
{
    private readonly IListAdapter _baseAdapter;
    private readonly Color? _textColor;
    private readonly Color? _selectedTextColor;
    private readonly int _selectedPosition;

    public ColoredPickerAdapter(
        IListAdapter baseAdapter,
        Color? textColor,
        Color? selectedTextColor,
        int selectedPosition)
    {
        _baseAdapter = baseAdapter ?? throw new ArgumentNullException(nameof(baseAdapter));
        _textColor = textColor;
        _selectedTextColor = selectedTextColor;
        _selectedPosition = selectedPosition;
    }

    // Return the total number of items from the original adapter
    public override int Count => _baseAdapter.Count;

    // Return the data item at the specified position
    public override Java.Lang.Object? GetItem(int position) => _baseAdapter.GetItem(position);

    // Return the unique identifier for the item at the specified position
    public override long GetItemId(int position) => _baseAdapter.GetItemId(position);

    public override AndroidView? GetView(int position, AndroidView? convertView, AndroidViewGroup? parent)
    {
        // Get the view from the original adapter.
        var view = _baseAdapter.GetView(position, convertView, parent);

        // Customize the view if it's a CheckedTextView
        if (view is CheckedTextView textView)
        {
            var isSelected = position == _selectedPosition;

            // Apply the appropriate text color based on selection state
            var color = isSelected ? _selectedTextColor : _textColor;
            if (color is not null)
            {
                textView.SetTextColor(color.ToPlatform());
            }
        }

        return view;
    }
}
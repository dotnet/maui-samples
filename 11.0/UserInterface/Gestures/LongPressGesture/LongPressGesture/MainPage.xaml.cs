using LongPressGesture.ViewModels;

namespace LongPressGesture;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new LongPressViewModel();
    }

    // <docregion_longpressed_handler>
    void OnLongPressed(object? sender, LongPressedEventArgs e)
    {
        if (BindingContext is LongPressViewModel vm)
        {
            var position = e.GetPosition(InteractiveBox);
            vm.PositionText = position.HasValue
                ? $"Position: ({position.Value.X:F0}, {position.Value.Y:F0})"
                : "Position: unavailable";
        }
    }
    // </docregion_longpressed_handler>

    // <docregion_longpressing_handler>
    void OnLongPressing(object? sender, LongPressingEventArgs e)
    {
        if (BindingContext is LongPressViewModel vm)
        {
            vm.LongPressState = e.Status switch
            {
                GestureStatus.Started => "Status: Started — hold steady...",
                GestureStatus.Running => "Status: Running — still holding...",
                GestureStatus.Completed => "Status: Completed — gesture complete!",
                GestureStatus.Canceled => "Status: Canceled — gesture aborted",
                _ => $"Status: {e.Status}"
            };
        }
    }
    // </docregion_longpressing_handler>

    void OnMultiTouchLongPressed(object? sender, LongPressedEventArgs e)
    {
        MultiTouchStatus.Text = "Two-finger long press detected!";
        MultiTouchBox.Color = Colors.Orange;
    }

    async void OnImageTapped(object? sender, TappedEventArgs e)
    {
        await DisplayAlert("Tap Detected", "You tapped the .NET bot!", "OK");
    }

    void OnImageLongPressed(object? sender, LongPressedEventArgs e)
    {
        if (BindingContext is LongPressViewModel vm)
        {
            var position = e.GetPosition(sender as Element);
            vm.ImageStatus = position.HasValue
                ? $"Long pressed at ({position.Value.X:F0}, {position.Value.Y:F0}) — context menu here!"
                : "Long pressed — context menu here!";
        }
    }
}

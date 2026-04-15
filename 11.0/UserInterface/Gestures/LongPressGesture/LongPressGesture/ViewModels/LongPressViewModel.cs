using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LongPressGesture.ViewModels;

// <docregion_viewmodel>
public partial class LongPressViewModel : ObservableObject
{
    [ObservableProperty]
    string gestureStatus = "Waiting for long press...";

    [ObservableProperty]
    string positionText = string.Empty;

    [ObservableProperty]
    Color boxColor = Colors.MediumPurple;

    [ObservableProperty]
    string longPressState = string.Empty;

    [ObservableProperty]
    string imageStatus = "Tap or long press the image";

    // <docregion_command>
    [RelayCommand]
    void LongPress(string source)
    {
        GestureStatus = $"✅ Long press completed on {source}!";
        BoxColor = GetRandomColor();
    }
    // </docregion_command>

    [RelayCommand]
    void Tap(string source)
    {
        ImageStatus = $"Tapped on {source}!";
    }

    public void OnBoxTapped()
    {
        GestureStatus = "👆 That was a tap, not a long press. Hold longer!";
    }

    static Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromRgb(
            random.Next(80, 220),
            random.Next(80, 220),
            random.Next(80, 220));
    }
}
// </docregion_viewmodel>

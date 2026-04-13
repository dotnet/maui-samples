using System.Runtime.CompilerServices;

namespace LongPressGesture.ViewModels;

// <docregion_viewmodel>
public class LongPressViewModel : INotifyPropertyChanged
{
    string _gestureStatus = "Waiting for long press...";
    string _positionText = string.Empty;
    Color _boxColor = Colors.MediumPurple;
    string _longPressState = string.Empty;
    string _imageStatus = "Tap or long press the image";

    public LongPressViewModel()
    {
        LongPressCommand = new Command<string>(OnLongPressed);
        TapCommand = new Command<string>(OnTapped);
    }

    // <docregion_command>
    /// <summary>
    /// Command executed when a long press gesture completes.
    /// </summary>
    public ICommand LongPressCommand { get; }
    // </docregion_command>

    public ICommand TapCommand { get; }

    /// <summary>
    /// Display text showing the current gesture status.
    /// </summary>
    public string GestureStatus
    {
        get => _gestureStatus;
        set => SetProperty(ref _gestureStatus, value);
    }

    /// <summary>
    /// Display text showing the gesture position.
    /// </summary>
    public string PositionText
    {
        get => _positionText;
        set => SetProperty(ref _positionText, value);
    }

    /// <summary>
    /// The background color of the interactive BoxView.
    /// </summary>
    public Color BoxColor
    {
        get => _boxColor;
        set => SetProperty(ref _boxColor, value);
    }

    /// <summary>
    /// Text representation of the current gesture state.
    /// </summary>
    public string LongPressState
    {
        get => _longPressState;
        set => SetProperty(ref _longPressState, value);
    }

    /// <summary>
    /// Status text for the image interaction demo.
    /// </summary>
    public string ImageStatus
    {
        get => _imageStatus;
        set => SetProperty(ref _imageStatus, value);
    }

    void OnLongPressed(string source)
    {
        GestureStatus = $"✅ Long press completed on {source}!";
        BoxColor = GetRandomColor();
    }

    void OnTapped(string source)
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

    bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;
        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
// </docregion_viewmodel>

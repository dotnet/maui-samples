namespace RpnCalculator;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Subscribe to SizeChanged event when page appears
        SizeChanged += OnSizeChanged;
        
        // Set initial layout based on current dimensions
        OnSizeChanged(this, EventArgs.Empty);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Unsubscribe from SizeChanged event when page disappears
        SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        portrait.IsVisible = !(landscape.IsVisible = Width > Height);
    }
}

namespace FlyoutPageNavigation;

public partial class MainPage : FlyoutPage
{
    public MainPage()
    {
        InitializeComponent();

        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        }
    }

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is FlyoutPageItem item)
        {
            Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType)!);
            flyoutPage.collectionView.SelectedItem = null;
            IsPresented = false;
        }
    }
}
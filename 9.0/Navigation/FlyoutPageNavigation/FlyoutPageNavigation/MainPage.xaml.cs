namespace FlyoutPageNavigation;

public partial class MainPage : FlyoutPage
{
    public MainPage()
    {
        InitializeComponent();

        flyoutPage.listView.ItemSelected += OnItemSelected;

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        }
    }

    void OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        var item = e.SelectedItem as FlyoutPageItem;
        if (item != null)
        {
            Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType)!);
            flyoutPage.listView.SelectedItem = null;
            IsPresented = false;
        }
    }
}
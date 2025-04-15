namespace FlyoutPageSample;

public partial class AppFlyout : FlyoutPage
{
	public AppFlyout()
	{
		InitializeComponent();

        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item != null)
        {
            Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));

            // Checking ShouldShowSplitMode is to fix the issue: https://github.com/dotnet/maui-samples/issues/219
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            {
                IsPresented = false;
            }
        }
    }
}
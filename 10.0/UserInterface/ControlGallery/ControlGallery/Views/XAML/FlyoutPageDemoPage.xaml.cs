using System.Collections;

namespace ControlGallery.Views.XAML
{
    public partial class FlyoutPageDemoPage : FlyoutPage
    {
        public FlyoutPageDemoPage()
        {
            InitializeComponent();

            collectionView.SelectedItem = (collectionView.ItemsSource as IList)?[0];
        }

        void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Show the detail page.
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            {
                IsPresented = false;
            }
        }
    }
}

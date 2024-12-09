using System.Collections;

namespace ControlGallery.Views.XAML
{
    public partial class FlyoutPageDemoPage : FlyoutPage
    {
        public FlyoutPageDemoPage()
        {
            InitializeComponent();

            listView.SelectedItem = (listView.ItemsSource as IList)?[0];
        }

        void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Show the detail page.
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            {
                IsPresented = false;
            }
        }
    }
}

using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class IncrementalLoadingPage : ContentPage
    {
        public IncrementalLoadingPage()
        {
            InitializeComponent();
            BindingContext = new AnimalsViewModel();
        }

        void OnCollectionViewRemainingItemsThresholdReached(object sender, EventArgs e)
        {
            // Retrieve more data here, or via the RemainingItemsThresholdReachedCommand.
            // This sample retrieves more data using the RemainingItemsThresholdReachedCommand.
        }
    }
}

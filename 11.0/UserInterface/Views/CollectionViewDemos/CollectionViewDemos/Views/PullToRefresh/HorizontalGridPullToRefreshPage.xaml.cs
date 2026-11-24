using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class HorizontalGridPullToRefreshPage : ContentPage
    {
        public HorizontalGridPullToRefreshPage()
        {
            InitializeComponent();
            BindingContext = new AnimalsViewModel();
        }
    }
}

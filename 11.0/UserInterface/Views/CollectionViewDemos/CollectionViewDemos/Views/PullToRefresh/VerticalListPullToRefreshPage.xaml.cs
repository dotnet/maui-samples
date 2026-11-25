using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListPullToRefreshPage : ContentPage
    {
        public VerticalListPullToRefreshPage()
        {
            InitializeComponent();
            BindingContext = new AnimalsViewModel();
        }
    }
}

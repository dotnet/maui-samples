using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class EmptyViewWithViewsFilteredPage : ContentPage
    {
        public EmptyViewWithViewsFilteredPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

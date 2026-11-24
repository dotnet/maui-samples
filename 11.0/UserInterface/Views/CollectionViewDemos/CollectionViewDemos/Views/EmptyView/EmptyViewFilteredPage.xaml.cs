using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class EmptyViewFilteredPage : ContentPage
    {
        public EmptyViewFilteredPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

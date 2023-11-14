using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListSwipeContextItemsPage : ContentPage
    {
        public VerticalListSwipeContextItemsPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();            
        }
    }
}

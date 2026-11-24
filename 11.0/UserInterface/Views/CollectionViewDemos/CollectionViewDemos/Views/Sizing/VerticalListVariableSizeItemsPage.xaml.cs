using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListVariableSizeItemsPage : ContentPage
    {
        public VerticalListVariableSizeItemsPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListHeaderFooterStringPage : ContentPage
    {
        public VerticalListHeaderFooterStringPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

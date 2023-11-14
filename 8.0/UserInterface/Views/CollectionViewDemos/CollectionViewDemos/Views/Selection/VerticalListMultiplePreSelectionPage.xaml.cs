using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListMultiplePreSelectionPage : ContentPage
    {
        public VerticalListMultiplePreSelectionPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

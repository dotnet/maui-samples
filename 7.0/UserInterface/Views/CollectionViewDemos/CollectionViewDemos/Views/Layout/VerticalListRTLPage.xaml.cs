using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListRTLPage : ContentPage
    {
        public VerticalListRTLPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

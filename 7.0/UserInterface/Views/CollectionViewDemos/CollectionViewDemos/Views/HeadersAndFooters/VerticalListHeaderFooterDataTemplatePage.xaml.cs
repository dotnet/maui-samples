using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListHeaderFooterDataTemplatePage : ContentPage
    {
        public VerticalListHeaderFooterDataTemplatePage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

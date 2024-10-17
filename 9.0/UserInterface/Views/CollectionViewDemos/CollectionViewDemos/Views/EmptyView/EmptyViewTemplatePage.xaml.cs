using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class EmptyViewTemplatePage : ContentPage
    {
        public EmptyViewTemplatePage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }
    }
}

using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListEmptyGroupsPage : ContentPage
    {
        public VerticalListEmptyGroupsPage()
        {
            InitializeComponent();
            BindingContext = new GroupedAnimalsViewModel(true);
        }
    }
}

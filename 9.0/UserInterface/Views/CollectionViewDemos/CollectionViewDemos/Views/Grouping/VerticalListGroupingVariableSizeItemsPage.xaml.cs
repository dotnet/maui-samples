using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class VerticalListGroupingVariableSizeItemsPage : ContentPage
    {
        public VerticalListGroupingVariableSizeItemsPage()
        {
            InitializeComponent();
            BindingContext = new GroupedAnimalsViewModel();
        }
    }
}

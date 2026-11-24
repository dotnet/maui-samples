using CollectionViewDemos.ViewModels;

namespace CollectionViewDemos.Views
{
    public partial class EmptyViewLoadSimulationPage : ContentPage
    {
        public EmptyViewLoadSimulationPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(2000);
            collectionView.ItemsSource = (BindingContext as MonkeysViewModel).Monkeys;
        }
    }
}

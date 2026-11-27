namespace DataBindingDemos
{
    public partial class MonkeysPage : ContentPage
    {
        public MonkeysPage()
        {
            InitializeComponent();
            BindingContext = new MonkeysViewModel();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Monkey monkey)
            {
                ((CollectionView)sender).SelectedItem = null;
                var page = new MonkeyDetailsPage();
                page.BindingContext = monkey;
                await Navigation.PushAsync(page);
            }
        }
    }
}

namespace SwipeViewDemos
{
    public partial class SwipeViewResourcePage : ContentPage
    {
        public SwipeViewResourcePage()
        {
            InitializeComponent();
        }

        async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("SwipeView", "Delete invoked.", "OK");
        }

        async void OnFavoriteSwipeItemInvoked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("SwipeView", "Favorite invoked.", "OK");
        }
    }
}


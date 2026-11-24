namespace SwipeViewDemos
{
    public partial class SwipeViewDirectionPage : ContentPage
    {
        public SwipeViewDirectionPage()
        {
            InitializeComponent();
        }

        async void OnFavoriteSwipeItemInvoked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("SwipeView", "Favorite invoked.", "OK");
        }

        async void OnShareSwipeItemInvoked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("SwipeView", "Share invoked.", "OK");
        }

        async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("SwipeView", "Delete invoked.", "OK");
        }
    }
}

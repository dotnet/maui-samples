namespace ThemingDemo
{
    public partial class UserDetailsPage : ContentPage
    {
        public UserDetailsPage()
        {
            InitializeComponent();
        }

        async void OnThemeToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ThemeSelectionPage());
        }
    }
}

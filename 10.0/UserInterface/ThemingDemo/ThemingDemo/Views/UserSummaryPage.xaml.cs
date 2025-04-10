﻿namespace ThemingDemo
{
    public partial class UserSummaryPage : ContentPage
    {
        public UserSummaryPage()
        {
            InitializeComponent();
        }

        async void OnNavigationInvoked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserDetailsPage());
        }

        async void OnThemeToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ThemeSelectionPage());
        }
    }
}

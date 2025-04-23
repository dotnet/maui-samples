namespace GameOfLife;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    async void OnHyperlinkTapped(object sender, EventArgs args)
    {
        Label label = (Label)sender;
        await Launcher.OpenAsync(label.Text);
    }

    async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}

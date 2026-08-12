namespace BackButtonAccessibility;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    async void OnViewOrderClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrderDetailPage());
    }
}

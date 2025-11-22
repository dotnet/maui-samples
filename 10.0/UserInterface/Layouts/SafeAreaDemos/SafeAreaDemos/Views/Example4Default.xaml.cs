using SafeAreaDemos.ViewModels;

namespace SafeAreaDemos.Views;

public partial class Example4Default : ContentPage
{
    public Example4Default()
    {
        InitializeComponent();
        BindingContext = new Example4ViewModel();
    }

    private async void OnBackButtonTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
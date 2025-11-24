namespace SafeAreaDemos.Views;

public partial class Example4Default : ContentPage
{
    public Example4Default()
    {
        InitializeComponent();
    }

    async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
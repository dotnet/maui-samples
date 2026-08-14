namespace SafeAreaDemos.Views;

public partial class Example1EdgeToEdge : ContentPage
{
	public Example1EdgeToEdge()
	{
		InitializeComponent();
	}

	async void Button_Clicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}

namespace SafeAreaDemos.Views;

public partial class Example2RespectAll : ContentPage
{
	public Example2RespectAll()
	{
		InitializeComponent();
	}

	async void Button_Clicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}
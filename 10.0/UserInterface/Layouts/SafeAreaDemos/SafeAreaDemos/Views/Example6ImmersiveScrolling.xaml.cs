namespace SafeAreaDemos.Views;

public partial class Example6ImmersiveScrolling : ContentPage
{
	public Example6ImmersiveScrolling()
	{
		InitializeComponent();
	}
	async void Button_Clicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}

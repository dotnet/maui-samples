namespace SafeAreaDemos.Views;

public partial class Example1EdgeToEdge : ContentPage
{
	public Example1EdgeToEdge()
	{
		InitializeComponent();
	}
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		
		// Hide navigation bar for true edge-to-edge experience
		if (Parent is NavigationPage navPage)
		{
			NavigationPage.SetHasNavigationBar(this, false);
		}
	}
}

namespace WorkingWithColors;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnColorDemoClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"///{nameof(ColorDemo)}");
	}

	private async void OnColorsInXamlClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"///{nameof(ColorsInXaml)}");
	}
}

namespace Calculator;

public partial class SchemesMenu : ContentPage
{
	public SchemesMenu()
	{
		InitializeComponent();
	}

    //async void OnButtonClicked(object sender, EventArgs args)
    //{
    //    //await label.RelRotateTo(360, 1000);
    //}

    private void OnButtonClicked(Object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Navigation.PushAsync(new NewMain(button.BackgroundColor));

    }
}

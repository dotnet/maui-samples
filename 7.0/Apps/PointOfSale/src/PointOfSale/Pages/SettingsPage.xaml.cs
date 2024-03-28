namespace PointOfSale.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}


    void StateTrigger_IsActiveChanged(System.Object sender, System.EventArgs e)
    {
        Debug.WriteLine("active");
    }
}


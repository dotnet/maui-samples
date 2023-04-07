using Microsoft.Maui.Controls;

namespace MauiCustomRenderer;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    void Handle_Pressed(object sender, System.EventArgs e)
    {
        (sender as VisualElement).FadeTo(0, 100);
    }

    void Handle_Released(object sender, System.EventArgs e)
    {
        (sender as VisualElement).FadeTo(1, 200);
    }
}


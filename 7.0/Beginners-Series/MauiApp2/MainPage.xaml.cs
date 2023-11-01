using MauiApp2.ViewModel;

namespace MauiApp2;

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingConteaxt = vm;
	}
}


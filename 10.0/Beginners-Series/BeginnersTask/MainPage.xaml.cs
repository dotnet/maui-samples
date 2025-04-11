using BeginnersTasks.ViewModel;

namespace BeginnersTasks;

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}


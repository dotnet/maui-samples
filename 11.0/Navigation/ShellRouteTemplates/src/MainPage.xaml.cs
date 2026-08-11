using ShellRouteTemplates.ViewModels;

namespace ShellRouteTemplates;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

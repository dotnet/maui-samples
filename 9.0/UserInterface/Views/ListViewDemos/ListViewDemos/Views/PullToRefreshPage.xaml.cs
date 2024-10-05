using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class PullToRefreshPage : ContentPage
{
	public PullToRefreshPage()
	{
		InitializeComponent();
		BindingContext = new AnimalsViewModel();
	}
}
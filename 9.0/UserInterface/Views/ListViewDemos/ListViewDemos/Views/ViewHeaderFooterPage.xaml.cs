using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class ViewHeaderFooterPage : ContentPage
{
	public ViewHeaderFooterPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
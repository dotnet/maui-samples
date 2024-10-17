using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class TemplatedHeaderFooterPage : ContentPage
{
	public TemplatedHeaderFooterPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
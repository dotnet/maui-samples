using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class VerticalListPage : ContentPage
{
	public VerticalListPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
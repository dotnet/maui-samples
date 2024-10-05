using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class RightToLeftListPage : ContentPage
{
	public RightToLeftListPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
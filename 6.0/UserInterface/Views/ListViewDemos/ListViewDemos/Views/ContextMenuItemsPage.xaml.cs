using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class ContextMenuItemsPage : ContentPage
{
	public ContextMenuItemsPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
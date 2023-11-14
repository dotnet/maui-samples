using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class TextHeaderFooterPage : ContentPage
{
	public TextHeaderFooterPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
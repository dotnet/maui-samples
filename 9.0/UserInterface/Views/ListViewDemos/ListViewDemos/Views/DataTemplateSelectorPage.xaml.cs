using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class DataTemplateSelectorPage : ContentPage
{
	public DataTemplateSelectorPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}
}
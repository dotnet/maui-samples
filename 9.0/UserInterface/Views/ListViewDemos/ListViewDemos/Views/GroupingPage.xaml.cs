using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class GroupingPage : ContentPage
{
	public GroupingPage()
	{
		InitializeComponent();
		BindingContext = new GroupedAnimalsViewModel();
	}
}
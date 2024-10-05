using ListViewDemos.Models;
using ListViewDemos.ViewModels;
using System.Diagnostics;

namespace ListViewDemos;

public partial class ScrollByItemWithGroupingPage : ContentPage
{
	public ScrollByItemWithGroupingPage()
	{
		InitializeComponent();
		BindingContext = new GroupedAnimalsViewModel();
	}


    void OnButtonClicked(object sender, EventArgs e)
    {
        GroupedAnimalsViewModel viewModel = BindingContext as GroupedAnimalsViewModel;
        AnimalGroup group = viewModel.Animals.FirstOrDefault(a => a.Name == "Monkeys");
        Animal monkey = group.FirstOrDefault(m => m.Name == "Proboscis Monkey");
        listView.ScrollTo(monkey, group, (ScrollToPosition)enumPicker.SelectedItem, animateSwitch.IsToggled);
    }

    void OnListViewScrolled(object sender, ScrolledEventArgs args)
    {
        Debug.WriteLine("ScrollX: ", args.ScrollX);
        Debug.WriteLine("ScrollY: ", args.ScrollY);
    }
}
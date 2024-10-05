using ListViewDemos.Models;
using ListViewDemos.ViewModels;
using System.Diagnostics;

namespace ListViewDemos;
public partial class ScrollByItemPage : ContentPage
{
	public ScrollByItemPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}

    void OnButtonClicked(object sender, EventArgs e)
    {
        MonkeysViewModel viewModel = BindingContext as MonkeysViewModel;
        Monkey monkey = viewModel.Monkeys.FirstOrDefault(m => m.Name == "Proboscis Monkey");
        listView.ScrollTo(monkey, (ScrollToPosition)enumPicker.SelectedItem, animateSwitch.IsToggled);
    }

    void OnListViewScrolled(object sender, ScrolledEventArgs args)
    {
        Debug.WriteLine("ScrollX: ", args.ScrollX);
        Debug.WriteLine("ScrollY: ", args.ScrollY);
    }
}

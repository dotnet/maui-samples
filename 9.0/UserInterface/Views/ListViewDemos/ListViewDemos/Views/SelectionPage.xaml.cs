using ListViewDemos.Models;
using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class SelectionPage : ContentPage
{
	public SelectionPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        currentSelectedItemLabel.Text = (args.SelectedItem as Monkey)?.Name;
    }
}

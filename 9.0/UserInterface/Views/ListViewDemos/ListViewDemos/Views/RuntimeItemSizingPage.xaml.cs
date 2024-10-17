using ListViewDemos.ViewModels;

namespace ListViewDemos;

public partial class RuntimeItemSizingPage : ContentPage
{
	public RuntimeItemSizingPage()
	{
		InitializeComponent();
		BindingContext = new MonkeysViewModel();
	}

	void OnImageTapped(object sender, EventArgs args)
    {
		Image image = sender as Image;
		ViewCell viewCell = image.Parent.Parent as ViewCell;

		if (image.HeightRequest < 250)
        {
			image.HeightRequest = image.Height + 100;
			viewCell.ForceUpdateSize();
        }
    }
}
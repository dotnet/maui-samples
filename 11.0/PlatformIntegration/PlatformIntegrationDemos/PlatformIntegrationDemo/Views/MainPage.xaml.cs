using PlatformIntegrationDemo.Models;

namespace PlatformIntegrationDemo.Views;

public partial class MainPage : BasePage
{
	public MainPage()
	{
		InitializeComponent();
	}

    async void OnSampleTapped(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection?.FirstOrDefault() as SampleItem;
        if (item == null)
            return;

        await Navigation.PushAsync((Page)Activator.CreateInstance(item.PageType));

        // deselect Item
        ((CollectionView)sender).SelectedItem = null;
    }
}



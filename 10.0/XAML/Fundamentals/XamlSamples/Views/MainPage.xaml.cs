namespace XamlSamples;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        (sender as ListView).SelectedItem = null;

        if (args.SelectedItem != null)
        {
            PageDataViewModel pageData = args.SelectedItem as PageDataViewModel;
            Page page = (Page)Activator.CreateInstance(pageData.Type);
            await Navigation.PushAsync(page);
        }
    }
}

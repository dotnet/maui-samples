namespace XamlSamples;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        if (args.CurrentSelection.FirstOrDefault() is PageDataViewModel pageData)
        {
            ((CollectionView)sender).SelectedItem = null;
            Page page = (Page)Activator.CreateInstance(pageData.Type);
            await Navigation.PushAsync(page);
        }
    }
}

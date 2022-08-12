using VideoDemos.Controls;

namespace VideoDemos.Views;

public partial class SelectWebVideoPage : ContentPage
{
    public SelectWebVideoPage()
    {
        InitializeComponent();
    }

    void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string key = ((string)e.CurrentSelection.FirstOrDefault()).Replace(" ", "").Replace("'", "");
        video.Source = (UriVideoSource)Application.Current.Resources[key];
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        video.Handler?.DisconnectHandler();
    }
}
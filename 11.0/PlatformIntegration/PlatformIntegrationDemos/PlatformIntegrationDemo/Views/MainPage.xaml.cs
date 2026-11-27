using PlatformIntegrationDemo.Models;

namespace PlatformIntegrationDemo.Views;

public partial class MainPage : BasePage
{
    public MainPage()
    {
        InitializeComponent();
    }

    async void OnSampleTapped(object sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is SampleItem item)
            await Navigation.PushAsync((Page)Activator.CreateInstance(item.PageType));
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Give the list more room on short screens, such as a phone in landscape.
        hero.IsVisible = height >= 560;
    }
}

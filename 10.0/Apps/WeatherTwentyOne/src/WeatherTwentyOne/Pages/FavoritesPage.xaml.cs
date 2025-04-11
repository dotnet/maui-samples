using WeatherTwentyOne.ViewModels;

namespace WeatherTwentyOne.Pages;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage()
    {
        InitializeComponent();

        BindingContext = new FavoritesViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(300);
        TransitionIn();
    }

    async void TransitionIn()
    {
        foreach (var item in tiles)
        {
            await item.FadeTo(1, 800);
            await Task.Delay(50);
        }
    }

    int tileCount = 0;
    List<Border> tiles = new List<Border>();
    void OnHandlerChanged(object sender, EventArgs e)
    {
        Border b = (Border)sender;
        tiles.Add(b);
        tileCount++;
    }
}

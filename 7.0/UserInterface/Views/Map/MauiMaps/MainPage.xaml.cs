using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MauiMaps;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var hanaLoc = new Location(20.7557, -155.9880);

        MapSpan mapSpan = MapSpan.FromCenterAndRadius(hanaLoc, Distance.FromKilometers(3));
        map.MoveToRegion(mapSpan);

        map.Pins.Add(new Pin
        {
            Label = "Subscribe to Gerald's channel?",
            Location = new Location(50.8514, 5.6910),
        });

        map.Pins.Add(new Pin
        {
            Label = "Welcome to .NET MAUI!",
            Location = hanaLoc,
        });
    }
}


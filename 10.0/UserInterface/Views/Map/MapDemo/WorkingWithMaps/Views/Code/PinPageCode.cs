using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace WorkingWithMaps.Views.Code;

public class PinPageCode : ContentPage
{
    public PinPageCode()
    {
        Title = "Pins demo";

        Location location = new Location(36.9628066, -122.0194722);
        MapSpan mapSpan = new MapSpan(location, 0.01, 0.01);

        Map map = new Map(mapSpan);

        Pin pin = new Pin
        {
            Label = "Santa Cruz",
            Address = "The city with a boardwalk",
            Type = PinType.Place,
            Location = location
        };
        map.Pins.Add(pin);

        Button button = new Button { Text = "Add more pins" };
        button.Clicked += (sender, e) =>
        {
            Pin boardwalkPin = new Pin
            {
                Location = new Location(36.9641949, -122.0177232),
                Label = "Boardwalk",
                Address = "Santa Cruz",
                Type = PinType.Place
            };

            boardwalkPin.MarkerClicked += async (s, args) =>
            {
                args.HideInfoWindow = true;
                string pinName = ((Pin)s).Label;
                await DisplayAlert("Pin Clicked", $"{pinName} was clicked.", "Ok");
            };

            Pin wharfPin = new Pin
            {
                Location = new Location(36.9571571, -122.0173544),
                Label = "Wharf",
                Address = "Santa Cruz",
                Type = PinType.Place
            };

            wharfPin.InfoWindowClicked += async (s, args) =>
            {
                string pinName = ((Pin)s).Label;
                await DisplayAlert("Info Window Clicked", $"The info window was clicked for {pinName}.", "Ok");
            };

            map.Pins.Add(boardwalkPin);
            map.Pins.Add(wharfPin);
        };

        StackLayout sl = new StackLayout { Margin = new Thickness(10) };
        sl.Add(map);
        sl.Add(button);
        Content = sl;
    }
}

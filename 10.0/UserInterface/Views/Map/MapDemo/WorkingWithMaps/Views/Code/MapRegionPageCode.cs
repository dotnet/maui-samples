using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace WorkingWithMaps.Views.Code;

public class MapRegionPageCode : ContentPage
{
    public MapRegionPageCode()
    {
        Title = "Map region demo";

        Location location = new Location(36.9628066, -122.0194722);
        MapSpan mapSpan = new MapSpan(location, 0.01, 0.01);
        //MapSpan mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(0.444));

        Map map = new Map(mapSpan);

        StackLayout sl = new StackLayout { Margin = new Thickness(10) };
        sl.Add(map);
        Content = sl;
    }
}

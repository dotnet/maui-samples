using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace WorkingWithMaps.Views.Code
{
    public class CirclePageCode : ContentPage
    {
        public CirclePageCode()
        {
            Map map = new Map();

            Location location = new Location(37.79752, -122.40183);
            Pin pin = new Pin
            {
                Label = "Microsoft San Francisco",
                Address = "1355 Market St, San Francisco CA",
                Type = PinType.Place,
                Location = location
            };
            map.Pins.Add(pin);

            Circle circle = new Circle
            {
                Center = location,
                Radius = new Distance(250),
                StrokeColor = Color.FromArgb("#88FF0000"),
                StrokeWidth = 8,
                FillColor = Color.FromArgb("#88FFC0CB")
            };
            map.MapElements.Add(circle);

            Title = "Circle demo";
            Content = map;

            map.MoveToRegion(new MapSpan(location, 0.01, 0.01));
        }
    }
}

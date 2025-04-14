using Microsoft.Maui.Devices.Sensors;

namespace WorkingWithMaps.Views.Code;

public class GeocoderPageCode : ContentPage
{
    Label l = new Label();

    public GeocoderPageCode()
    {
        Title = "Geocoder demo";

        var b1 = new Button { Text = "Reverse geocode '37.808, -122.432'" };
        b1.Clicked += async (sender, e) =>
        {
            Location fortMasonLocation = new Location(37.8044866, -122.4324132);
            IEnumerable<Placemark> possibleAddresses = await Geocoding.Default.GetPlacemarksAsync(fortMasonLocation);
            foreach (var a in possibleAddresses)
            {
                l.Text += a + "\n";
            }
        };

        var b2 = new Button { Text = "Geocode '394 Pacific Ave'" };
        b2.Clicked += async (sender, e) =>
        {
            var address = "394 Pacific Ave, San Francisco, California";
            IEnumerable<Location> approximateLocations = await Geocoding.Default.GetLocationsAsync(address);
            foreach (var p in approximateLocations)
            {
                l.Text += p.Latitude + ", " + p.Longitude + "\n";
            }
        };

        StackLayout sl = new StackLayout { Padding = new Thickness(0, 20, 0, 0) };
        sl.Add(b2);
        sl.Add(b1);
        sl.Add(l);
        Content = sl;
    }
}
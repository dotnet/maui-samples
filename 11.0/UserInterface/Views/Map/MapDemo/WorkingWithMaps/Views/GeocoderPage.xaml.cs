using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;

namespace WorkingWithMaps.Views;

public partial class GeocoderPage : ContentPage
{
    public GeocoderPage()
    {
        InitializeComponent();
    }

    async void OnGeocodeButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(geocodeEntry.Text))
        {
            string address = geocodeEntry.Text;
            IEnumerable<Location> approximateLocations = await Geocoding.Default.GetLocationsAsync(address);
            Location location = approximateLocations.FirstOrDefault();
            geocodedOutputLabel.Text = $"{location.Latitude}, {location.Longitude}";
        }
    }

    async void OnReverseGeocodeButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(reverseGeocodeEntry.Text))
        {
            string[] coordinates = reverseGeocodeEntry.Text.Split(',');
            double? latitude = Convert.ToDouble(coordinates.FirstOrDefault());
            double? longitude = Convert.ToDouble(coordinates.Skip(1).FirstOrDefault());

            if (latitude != null && longitude != null)
            {
                Location location = new Location(latitude.Value, longitude.Value);

                Location fortMasonLocation = new Location(37.8044866, -122.4324132);
                IEnumerable<Placemark> possibleAddresses = await Geocoding.Default.GetPlacemarksAsync(location);
                Placemark placemark = possibleAddresses?.FirstOrDefault();
                if (placemark != null)
                    reverseGeocodedOutputLabel.Text = $"Thoroughfare: {placemark.Thoroughfare}, Locality: {placemark.Locality}, ZipCode: {placemark.PostalCode}";
                else
                    reverseGeocodedOutputLabel.Text = string.Empty;
            }
        }
    }
}

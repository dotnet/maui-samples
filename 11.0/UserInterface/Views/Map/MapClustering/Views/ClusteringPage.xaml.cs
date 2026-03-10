using Microsoft.Maui.Controls.Maps;

namespace MapClustering.Views;

public partial class ClusteringPage : ContentPage
{
    public ClusteringPage()
    {
        InitializeComponent();
        map.ClusterClicked += OnClusterClicked;
    }

    // <docregion_add_clustered_pins>
    void OnAddPinsClicked(object sender, EventArgs e)
    {
        map.Pins.Clear();

        // Seattle area coffee shops - same clustering identifier
        var coffeeShops = new[]
        {
            (47.6097, -122.3331, "Pike Place Coffee"),
            (47.6080, -122.3358, "Waterfront Roasters"),
            (47.6115, -122.3375, "Belltown Beans"),
            (47.6062, -122.3350, "Pioneer Square Espresso"),
            (47.6050, -122.3295, "International District Tea"),
            (47.6138, -122.3440, "Queen Anne Café"),
            (47.6150, -122.3200, "Capitol Hill Brew"),
            (47.6200, -122.3210, "Eastlake Coffee"),
        };

        foreach (var (lat, lon, name) in coffeeShops)
        {
            map.Pins.Add(new Pin
            {
                Label = name,
                Address = "Seattle, WA",
                Location = new Location(lat, lon),
                Type = PinType.Place,
                ClusteringIdentifier = "coffee"
            });
        }

        // Parks - different clustering identifier so they cluster separately
        var parks = new[]
        {
            (47.6064, -122.3325, "Occidental Square"),
            (47.6114, -122.3426, "Victor Steinbrueck Park"),
            (47.6130, -122.3160, "Cal Anderson Park"),
            (47.6285, -122.3417, "Kerry Park"),
        };

        foreach (var (lat, lon, name) in parks)
        {
            map.Pins.Add(new Pin
            {
                Label = name,
                Address = "Seattle, WA",
                Location = new Location(lat, lon),
                Type = PinType.SavedPin,
                ClusteringIdentifier = "parks"
            });
        }

        statusLabel.Text = $"Added {coffeeShops.Length} coffee shops and {parks.Length} parks. Zoom out to see clusters.";
    }
    // </docregion_add_clustered_pins>

    // <docregion_cluster_clicked>
    async void OnClusterClicked(object? sender, ClusterClickedEventArgs e)
    {
        string pinNames = string.Join(", ", e.Pins.Select(p => p.Label));
        await DisplayAlertAsync(
            $"Cluster ({e.Pins.Count} pins)",
            $"Pins: {pinNames}\nLocation: {e.Location.Latitude:F4}, {e.Location.Longitude:F4}",
            "OK");

        // Set Handled to true to prevent default zoom-to-cluster behavior
        // e.Handled = true;
    }
    // </docregion_cluster_clicked>

    void OnToggleClusteringClicked(object sender, EventArgs e)
    {
        map.IsClusteringEnabled = !map.IsClusteringEnabled;
        statusLabel.Text = $"Clustering is {(map.IsClusteringEnabled ? "enabled" : "disabled")}.";
    }
}

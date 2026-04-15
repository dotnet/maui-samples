using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MapClustering.Views;

public partial class ClusteringPage : ContentPage
{
    private readonly Circle _circle;
    private readonly Polygon _polygon;
    private int _longPressPinCount;
    private bool _overlaysVisible = true;

    private static readonly Location SeattleCenter = new(47.6062, -122.3321);

    // #docregion pin_data
    private static readonly (string Label, double Lat, double Lon)[] SeattlePins =
    [
        ("Pike Place Market", 47.6097, -122.3425),
        ("Space Needle", 47.6205, -122.3493),
        ("Pioneer Square", 47.6019, -122.3343),
        ("Capitol Hill", 47.6253, -122.3222),
        ("Fremont", 47.6510, -122.3505),
        ("Ballard", 47.6677, -122.3840),
        ("University District", 47.6615, -122.3130),
        ("Chinatown", 47.5982, -122.3267),
        ("South Lake Union", 47.6249, -122.3381),
        ("Queen Anne", 47.6370, -122.3570),
        ("Belltown", 47.6145, -122.3470),
        ("Wallingford", 47.6583, -122.3352),
    ];
    // #enddocregion pin_data

    private static readonly HashSet<string> CustomIconPins =
    [
        "Space Needle", "Pike Place Market", "Capitol Hill"
    ];

    public ClusteringPage()
    {
        InitializeComponent();

        // #docregion circle_overlay
        _circle = new Circle
        {
            Center = SeattleCenter,
            Radius = new Distance(800),
            FillColor = Color.FromArgb("#302196F3"),
            StrokeColor = Color.FromArgb("#802196F3"),
            StrokeWidth = 3,
            ZIndex = 1
        };
        _circle.CircleClicked += OnCircleClicked;
        // #enddocregion circle_overlay

        // #docregion polygon_overlay
        _polygon = new Polygon
        {
            FillColor = Color.FromArgb("#30FF9800"),
            StrokeColor = Color.FromArgb("#80FF9800"),
            StrokeWidth = 3,
            ZIndex = 2
        };
        _polygon.Geopath.Add(new Location(47.640, -122.370));
        _polygon.Geopath.Add(new Location(47.635, -122.355));
        _polygon.Geopath.Add(new Location(47.645, -122.355));
        _polygon.PolygonClicked += OnPolygonClicked;
        // #enddocregion polygon_overlay

        map.MapElements.Add(_circle);
        map.MapElements.Add(_polygon);

        map.ClusterClicked += OnClusterClicked;

        AddDefaultPins();

        // #docregion move_to_region
        map.MoveToRegion(MapSpan.FromCenterAndRadius(SeattleCenter, Distance.FromKilometers(4)));
        // #enddocregion move_to_region
    }

    // #docregion add_clustered_pins
    private void AddDefaultPins()
    {
        map.Pins.Clear();

        foreach (var (label, lat, lon) in SeattlePins)
        {
            var pin = new Pin
            {
                Label = label,
                Location = new Location(lat, lon),
                ClusteringIdentifier = "seattle-places"
            };

            // #docregion custom_pin_icon
            if (CustomIconPins.Contains(label))
                pin.ImageSource = ImageSource.FromFile("dotnet_bot.png");
            // #enddocregion custom_pin_icon

            pin.MarkerClicked += OnPinClicked;
            map.Pins.Add(pin);
        }
    }
    // #enddocregion add_clustered_pins

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        statusLabel.Text = $"Map tapped at ({e.Location.Latitude:F4}, {e.Location.Longitude:F4})";
    }

    // #docregion map_long_clicked
    private void OnMapLongClicked(object? sender, MapClickedEventArgs e)
    {
        _longPressPinCount++;
        var pin = new Pin
        {
            Label = $"Dropped Pin #{_longPressPinCount}",
            Location = e.Location,
            ClusteringIdentifier = "user-pins"
        };
        pin.MarkerClicked += OnPinClicked;
        map.Pins.Add(pin);

        statusLabel.Text = $"Long press! Dropped pin at ({e.Location.Latitude:F4}, {e.Location.Longitude:F4})";
    }
    // #enddocregion map_long_clicked

    private void OnPinClicked(object? sender, PinClickedEventArgs e)
    {
        if (sender is Pin pin)
            statusLabel.Text = $"Pin tapped: {pin.Label}";
    }

    // #docregion cluster_clicked
    async void OnClusterClicked(object? sender, ClusterClickedEventArgs e)
    {
        string pinNames = string.Join(", ", e.Pins.Select(p => p.Label));
        await DisplayAlertAsync(
            $"Cluster ({e.Pins.Count} pins)",
            $"Pins: {pinNames}\nLocation: {e.Location.Latitude:F4}, {e.Location.Longitude:F4}",
            "OK");
    }
    // #enddocregion cluster_clicked

    // #docregion circle_clicked
    private void OnCircleClicked(object? sender, EventArgs e)
    {
        statusLabel.Text = "Circle overlay tapped! (CircleClicked event)";
        _circle.FillColor = Color.FromArgb("#504CAF50");
        Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () =>
            _circle.FillColor = Color.FromArgb("#302196F3"));
    }
    // #enddocregion circle_clicked

    // #docregion polygon_clicked
    private void OnPolygonClicked(object? sender, EventArgs e)
    {
        statusLabel.Text = "Polygon overlay tapped! (PolygonClicked event)";
        _polygon.FillColor = Color.FromArgb("#50E91E63");
        Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () =>
            _polygon.FillColor = Color.FromArgb("#30FF9800"));
    }
    // #enddocregion polygon_clicked

    void OnToggleClusteringClicked(object? sender, EventArgs e)
    {
        map.IsClusteringEnabled = !map.IsClusteringEnabled;
        clusterToggleButton.Text = map.IsClusteringEnabled ? "Clustering: On" : "Clustering: Off";
        statusLabel.Text = map.IsClusteringEnabled
            ? "Pin clustering enabled — zoom out to see clusters"
            : "Pin clustering disabled — all pins shown individually";
    }

    // #docregion toggle_overlays
    private void OnToggleOverlaysClicked(object? sender, EventArgs e)
    {
        _overlaysVisible = !_overlaysVisible;
        _circle.IsVisible = _overlaysVisible;
        _polygon.IsVisible = _overlaysVisible;
        overlayToggleButton.Text = _overlaysVisible ? "Toggle Overlays" : "Show Overlays";
        statusLabel.Text = _overlaysVisible
            ? "Overlays visible (MapElement.IsVisible = true)"
            : "Overlays hidden (MapElement.IsVisible = false)";
    }
    // #enddocregion toggle_overlays

    private void OnResetPinsClicked(object? sender, EventArgs e)
    {
        _longPressPinCount = 0;
        AddDefaultPins();
        map.MoveToRegion(MapSpan.FromCenterAndRadius(SeattleCenter, Distance.FromKilometers(4)));
        statusLabel.Text = "Pins reset to default Seattle locations";
    }
}

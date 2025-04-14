﻿using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace WorkingWithMaps.Views.Code;

public class PolygonsPageCode : ContentPage
{
    Map map;
    Polyline interstateBridge;
    Polygon msWest;
    Polygon msEast;

    public PolygonsPageCode()
    {
        Title = "Polygon/Polyline Code demo";

        map = new Map();

        Button polygonButton = new Button
        {
            Text = "Show Campus (Polygons)"
        };
        polygonButton.Clicked += AddPolygonsClicked;

        Button polylineButton = new Button
        {
            Text = "Show Access Road (Polyline)"
        };
        polylineButton.Clicked += AddPolylineClicked;

        Button clearButton = new Button
        {
            Text = "Clear All"
        };
        clearButton.Clicked += ClearClicked;

        msWest = new Polygon
        {
            StrokeColor = Color.FromArgb("#FF9900"),
            StrokeWidth = 8,
            FillColor = Color.FromArgb("#88FF9900"),
            Geopath =
            {
                new Location(47.6458676, -122.1356007),
                new Location(47.6458097, -122.142789),
                new Location(47.6367593, -122.1428104),
                new Location(47.6368027, -122.1398707),
                new Location(47.6380172, -122.1376177),
                new Location(47.640663, -122.1352359),
                new Location(47.6426148, -122.1347209),
                new Location(47.6458676, -122.1356007)
            }
        };

        msEast = new Polygon
        {
            StrokeColor = Color.FromArgb("#1BA1E2"),
            StrokeWidth = 8,
            FillColor = Color.FromArgb("#881BA1E2"),
            Geopath =
            {
                new Location(47.6368678, -122.137305),
                new Location(47.6368894, -122.134655),
                new Location(47.6359424, -122.134655),
                new Location(47.6359496, -122.1325521),
                new Location(47.6424124, -122.1325199),
                new Location(47.642463, -122.1338932),
                new Location(47.6406414, -122.1344833),
                new Location(47.6384943, -122.1361248),
                new Location(47.6372943, -122.1376912),
                new Location(47.6368678, -122.137305),
            }
        };

        interstateBridge = new Polyline
        {
            StrokeColor = Colors.Black,
            StrokeWidth = 12,
            Geopath =
            {
                new Location(47.6381401, -122.1317367),
                new Location(47.6381473, -122.1350841),
                new Location(47.6382847, -122.1353094),
                new Location(47.6384582, -122.1354703),
                new Location(47.6401136, -122.1360819),
                new Location(47.6403883, -122.1364681),
                new Location(47.6407426, -122.1377019),
                new Location(47.6412558, -122.1404056),
                new Location(47.6414148, -122.1418647),
                new Location(47.6414654, -122.1432702)
            }
        };

        StackLayout sl = new StackLayout();
        sl.Add(map);
        sl.Add(polygonButton);
        sl.Add(polylineButton);
        sl.Add(clearButton);
        Content = sl;

        map.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                new Location(47.640663, -122.1376177), Distance.FromMiles(1)));
    }

    void AddPolylineClicked(object sender, EventArgs e)
    {
        if (!map.MapElements.Contains(interstateBridge))
        {
            map.MapElements.Add(interstateBridge);
        }
    }

    void AddPolygonsClicked(object sender, EventArgs e)
    {
        if (!map.MapElements.Contains(msWest))
        {
            map.MapElements.Add(msWest);
        }

        if (!map.MapElements.Contains(msEast))
        {
            map.MapElements.Add(msEast);
        }
    }

    void ClearClicked(object sender, EventArgs e)
    {
        map.MapElements.Remove(msWest);
        map.MapElements.Remove(msEast);
        map.MapElements.Remove(interstateBridge);
    }
}
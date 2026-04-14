---
name: .NET MAUI - Map Features
description: Demonstrates new Maps features in .NET MAUI 11 Preview 3, including pin clustering, custom pin icons, overlay click events, long press to drop pins, and MapElement visibility toggling.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: userinterface-map-features
---

# Map Features

This sample demonstrates the Maps features introduced in .NET MAUI 11 for the Map control.

![.NET MAUI Map with pin clustering](images/clustering.png)

## Features

- **Pin clustering** — Set `Map.IsClusteringEnabled` to group nearby pins into cluster markers, with `Pin.ClusteringIdentifier` for separate clustering groups.
- **Cluster tap handling** — Handle the `Map.ClusterClicked` event to display cluster details or suppress the default zoom behavior.
- **Custom pin icons** — Use `Pin.ImageSource` to set any image as a pin marker.
- **Long press to drop pins** — Handle the `Map.MapLongClicked` event to add pins interactively.
- **Circle and Polygon overlay click events** — Handle `Circle.CircleClicked` and `Polygon.PolygonClicked` to respond to overlay taps.
- **Overlay visibility** — Toggle `MapElement.IsVisible` to show/hide overlays without removing them.
- **ZIndex layering** — Control overlay draw order with `MapElement.ZIndex`.
- **Animated map navigation** — Use `Map.MoveToRegion` with `MapSpan.FromCenterAndRadius` for smooth camera transitions.

## Prerequisites

- .NET 11 Preview 3 or later
- .NET MAUI workload installed
- Android: Google Maps API key configured in `AndroidManifest.xml`
- iOS/Mac Catalyst: No additional configuration required

## Build and run

1. Open `MapClustering.sln` in Visual Studio 2022 or later.
2. For Android, add your Google Maps API key to `Platforms/Android/AndroidManifest.xml`.
3. Select a target platform and run.

## Key APIs

- [`Map.IsClusteringEnabled`](https://learn.microsoft.com/dotnet/maui/user-interface/controls/map)
- [`Pin.ClusteringIdentifier`](https://learn.microsoft.com/dotnet/maui/user-interface/controls/map)
- [`Map.ClusterClicked`](https://learn.microsoft.com/dotnet/maui/user-interface/controls/map)
- `Pin.ImageSource`
- `Map.MapLongClicked`
- `Circle.CircleClicked`
- `Polygon.PolygonClicked`
- `MapElement.IsVisible`
- `MapElement.ZIndex`
- `Map.MoveToRegion`
- `MapSpan.FromCenterAndRadius`

## Platform support

| Platform | Supported |
|----------|-----------|
| Android | ✅ |
| iOS | ✅ |
| Mac Catalyst | ✅ |
| Windows | ❌ Not yet supported |

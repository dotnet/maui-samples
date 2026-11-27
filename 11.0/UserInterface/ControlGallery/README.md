---
name: .NET MAUI - Control gallery
description: This sample displays all of the controls available in .NET MAUI.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: userinterface-controlgallery
---

# Control gallery

The user interface of a .NET Multi-platform App UI (.NET MAUI) app is constructed of objects that map to the native controls of each target platform.

The main control groups used to create the user interface of a .NET MAUI app are pages, layouts, and views. A .NET MAUI page generally occupies the full screen or window. The page usually contains a layout, which contains views and possibly other layouts. Pages, layouts, and views derive from the `VisualElement` class. This class provides a variety of properties, methods, and events that are useful in derived classes.

This sample displays supported .NET MAUI controls, one per page. Deprecated ListView, TableView, and cell-only pages are not carried forward; use the CollectionView, Entry, Image, Switch, Label, and layout pages instead.

The home page groups the controls by category, and you can search or filter them. Every demo page uses the same header and card layout, in light and dark themes.

For more information about .NET MAUI controls, see [.NET MAUI controls](https://docs.microsoft.com/dotnet/maui/user-interface/controls/).

## Styling

All styling lives in resource dictionaries, so the demo pages contain little more than the control they demonstrate:

- `Resources/Styles/Colors.xaml` defines the color tokens. Most come in `Light` and `Dark` pairs, plus a shared accent palette.
- `Resources/Styles/Styles.xaml` defines implicit styles that theme every control, and explicit styles such as `Title1`, `Lead`, `Card`, and `DemoSurface` that pages opt into.
- The app uses the [Inter](https://rsms.me/inter/) typeface (SIL Open Font License) and [Material Icons Round](https://fonts.google.com/icons) (Apache License 2.0). Their license files are in `Resources/Fonts`.
- The animal images used by the CarouselView, CollectionView, IndicatorView, and SwipeView demos are 3D [Fluent Emoji](https://github.com/microsoft/fluentui-emoji) by Microsoft (MIT License). They're bundled with the app in `Resources/Images/Animals`, with their license file.

---
name: .NET MAUI - BoxView.Fill
description: "Demonstrates the new BoxView.Fill property in .NET MAUI 11, which accepts any Brush so gradients can paint a BoxView without a custom handler."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: views-boxviewfill
---
# BoxView.Fill

This sample demonstrates the new `Fill` property on `BoxView` introduced in .NET MAUI 11 ([dotnet/maui#31789](https://github.com/dotnet/maui/pull/31789)).

`Fill` is of type `Brush`, so a `BoxView` can be painted with a `SolidColorBrush`, `LinearGradientBrush`, `RadialGradientBrush`, or any other `Brush` — no custom handler required. This aligns `BoxView` with the other shape primitives. `BackgroundColor` still works for solid colors, but only `Fill` accepts brushes.

## Features demonstrated

- **SolidColorBrush** assigned to `BoxView.Fill`.
- **LinearGradientBrush** with two gradient stops painting a rectangular `BoxView`.
- **RadialGradientBrush** with three gradient stops painting a circular `BoxView`.
- **Fill vs BackgroundColor** comparison.

## Project structure

| File | Purpose |
|------|---------|
| `MainPage.xaml` | UI with four BoxView demos |
| `MainPage.xaml.cs` | Empty code-behind |

## Prerequisites

- .NET 11 Preview 5 or later
- .NET MAUI workload (`dotnet workload install maui`)

## Running the sample

```bash
dotnet build -t:Run -f net11.0-maccatalyst
```

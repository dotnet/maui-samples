---
name: .NET MAUI - LongPressGesture
description: "This sample demonstrates using the LongPressGestureRecognizer class to implement long press gesture recognition in .NET MAUI 11."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: gestures-longpressgesture
---
# LongPressGesture

This sample demonstrates the `LongPressGestureRecognizer` introduced in .NET MAUI 11 ([dotnet/maui#33432](https://github.com/dotnet/maui/pull/33432)).

## Features demonstrated

- **Long press with Command binding** — A `BoxView` that changes color when the user presses and holds for a configurable duration.
- **MinimumPressDuration** — Custom hold duration (750 ms in this sample).
- **AllowableMovement** — Movement threshold that cancels the gesture if exceeded.
- **NumberOfTouchesRequired** — Requiring two fingers for a long press (iOS / Mac Catalyst only).
- **State tracking** — Real-time feedback via the `LongPressing` event and `GestureStatus` (`Started`, `Running`, `Completed`, `Canceled`).
- **Position detection** — Using `GetPosition()` to report where the gesture occurred.
- **Gesture coexistence** — Combining `TapGestureRecognizer` and `LongPressGestureRecognizer` on the same `Image` element.

## Project structure

| File | Purpose |
|------|---------|
| `MainPage.xaml` | UI with three interactive demos |
| `MainPage.xaml.cs` | Event handlers for `LongPressed` and `LongPressing` |
| `ViewModels/LongPressViewModel.cs` | MVVM ViewModel with commands and bindable properties |

## Prerequisites

- .NET 11 Preview 3 or later
- .NET MAUI workload (`dotnet workload install maui`)

## Running the sample

```bash
dotnet build -t:Run -f net11.0-maccatalyst
```

## Platform notes

| Feature | Android | iOS / Mac Catalyst |
|---------|---------|-------------------|
| `MinimumPressDuration` | System default (~400 ms) | Fully configurable |
| `NumberOfTouchesRequired` | Always 1 | Configurable |
| State tracking | Completed / Canceled only | Full (Started / Running / Completed / Canceled) |

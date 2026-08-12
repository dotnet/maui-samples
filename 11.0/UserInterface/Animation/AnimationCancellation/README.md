---
name: .NET MAUI - Animation cancellation
description: "Demonstrates the new CancellationToken support on ViewExtensions animation methods in .NET MAUI 11."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: animation-cancellation
---
# Animation cancellation

This sample demonstrates the new `CancellationToken` overloads on the `ViewExtensions` animation methods introduced in .NET MAUI 11 ([dotnet/maui#33372](https://github.com/dotnet/maui/pull/33372)).

The async animation extensions — `FadeToAsync`, `TranslateToAsync`, `ScaleToAsync`, `RotateToAsync`, and friends — now accept a `CancellationToken`. They return `Task<bool>`: when the token fires, the in-flight animation aborts and the awaited result is `true` (versus `false` when the animation runs to completion). No exception is thrown on cancellation, so the call site simply checks the bool.

The non-token-based methods such as `FadeTo` are now marked `[Obsolete]` in favour of the new async overloads.

## Features demonstrated

- `Image.FadeToAsync(0, 5000, Easing.SinIn, token)` driven by a `CancellationTokenSource`.
- A Cancel button that calls `_cts.Cancel()` to abort the running animation.
- Inspecting the `Task<bool>` result to distinguish a cancellation from a natural finish.

## Project structure

| File | Purpose |
|------|---------|
| `MainPage.xaml` | UI with Image + Fade / Cancel / Reset buttons |
| `MainPage.xaml.cs` | Cancellation logic |

## Prerequisites

- .NET 11 Preview 5 or later
- .NET MAUI workload (`dotnet workload install maui`)

## Running the sample

```bash
dotnet build -t:Run -f net11.0-maccatalyst
```

## Try this

1. Tap **Fade out (5s)** to start a long, slow fade.
2. Tap **Cancel** before the fade finishes — the animation stops where it is and the status label reports the cancelled opacity.
3. Tap **Reset** to restore opacity and try again.

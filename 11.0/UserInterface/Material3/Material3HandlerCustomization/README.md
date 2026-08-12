---
name: .NET MAUI - Material 3 handler customization
description: "Demonstrates how to customise the now-public Android Material 3 handler types in .NET MAUI 11 by subclassing MauiMaterialEditText and wiring it through EditorHandler2.PlatformViewFactory."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: material3-handler-customization
---
# Material 3 handler customization

This sample demonstrates the now-public Android Material 3 helper types and Handler2 classes added in .NET MAUI 11 ([dotnet/maui#35323](https://github.com/dotnet/maui/pull/35323), closes [#35124](https://github.com/dotnet/maui/issues/35124)).

In prior previews, types like `MauiMaterialEditText`, `MauiMaterialContextThemeWrapper`, and the `Handler2` Editor / Picker / RadioButton / TimePicker / Label classes were `internal`, so apps couldn't subclass them or swap in their own platform view. .NET MAUI 11 makes the helper types and the Handler2 classes `public`, and adds a `PlatformViewFactory` hook so an app can construct its own subclass.

This sample subclasses `MauiMaterialEditText` to force every keystroke to upper case, then wires it into `EditorHandler2` via `PlatformViewFactory`.

## Features demonstrated

- `<UseMaterial3>true</UseMaterial3>` to opt in to the Material 3 handler set on Android.
- `MyMaterialEditText : MauiMaterialEditText` subclass that overrides `OnTextChanged`.
- `EditorHandler2.PlatformViewFactory = handler => new MyMaterialEditText(...)` to swap the platform view while preserving the default Material 3 theming, `ImeOptions`, `Gravity`, `TextAlignment`, and single-line / horizontal-scrolling setup.

## Material 3 Handler2 → platform view mapping

| Helper type | Handler |
|------|---------|
| `MauiMaterialEditText` | `EditorHandler2` |
| `MauiMaterialTextInputLayout` | `EntryHandler2` |
| `MauiMaterialSearchBarTextInputLayout` | `SearchBarHandler2` |
| `MauiMaterialDatePicker` | `DatePickerHandler2` |
| `MauiMaterialPicker` | `PickerHandler2` |
| `MauiMaterialTimePicker` | `TimePickerHandler2` |
| `MauiMaterialContextThemeWrapper` | (helper for theming a `Context`) |
| `MauiMaterialButton` | (used by `ButtonHandler` / `RadioButtonHandler2`) |

## Project structure

| File | Purpose |
|------|---------|
| `Material3HandlerCustomization.csproj` | Sets `<UseMaterial3>true</UseMaterial3>` |
| `MauiProgram.cs` | Calls the Android setup in an `#if ANDROID` block |
| `Platforms/Android/MyMaterialEditText.cs` | `MauiMaterialEditText` subclass that uppercases text |
| `Platforms/Android/MaterialEditorSetup.cs` | Sets `EditorHandler2.PlatformViewFactory` |
| `MainPage.xaml` | An `Editor` to demonstrate the customisation |

## Prerequisites

- .NET 11 Preview 5 or later
- .NET MAUI workload (`dotnet workload install maui`)

## Running the sample

```bash
dotnet build -t:Run -f net11.0-android
```

The customisation is Android-only — on iOS, Mac Catalyst, and Windows the page uses the default `Editor` handler.

## Try this

1. Build and run on Android (`-f net11.0-android`).
2. Tap into the `Editor` and start typing in mixed case.
3. Watch each character snap to upper case as it lands — that's `MyMaterialEditText.OnTextChanged` running.

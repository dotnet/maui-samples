# Picker Dialog Customization Sample

This .NET MAUI sample demonstrates how to create a custom Picker control with full dialog customization across all platforms (Android, iOS, macOS, Windows). It showcases platform-specific handler implementations to customize the native picker dialog's background color, text colors, and selected item styling.

## 🎨 Features

- **Custom Picker Control**: Extends the standard MAUI `Picker` with dialog styling capabilities
- **Dialog Background Color**: Customize the background color of the picker dialog/dropdown
- **Text Color Customization**: Set different colors for unselected and selected items
- **Multi-Platform Support**: Consistent API across Android, iOS, macOS, and Windows
- **Null-Safe Handling**: Gracefully falls back to platform-specific default colors when not specified
- **Platform-Specific Handlers**: Custom handlers for each platform using native controls
- **Instant Visual Feedback**: Optimized for smooth, delay-free color updates during selection

## 📁 Project Structure

```
PickerDemo/
├── Control/
│   ├── CustomPicker.cs              # Custom picker control with bindable properties
│   └── ICustomPicker.cs             # Interface definition
├── Handlers/
│   ├── CustomPickerHandler.cs       # Base handler with property mappers
│   ├── CustomPickerHandler.Android.cs   # Android implementation (AlertDialog)
│   ├── CustomPickerHandler.iOS.cs       # iOS/macOS implementation (UIPickerView)
│   └── CustomPickerHandler.Windows.cs   # Windows implementation (ComboBox)
├── MainPage.xaml                    # Demo page with picker examples
├── MauiProgram.cs                   # Handler registration
└── README.md
```

## 📱 Platform-Specific Implementations

### Android

- **Native Control**: `AppCompatAlertDialog` with custom adapter
- **Approach**: Reflection-based dialog access + adapter wrapping pattern
- **Key Feature**: `ColoredPickerAdapter` wraps original adapter to control item appearance

### iOS

- **Native Control**: `UIPickerView`
- **Approach**: Custom delegate with pending selection state
- **Key Feature**: Instant color updates with `CustomPickerViewDelegate`

### macOS (Catalyst)

- **Native Control**: `UIAlertController` with embedded `UIPickerView`
- **Approach**: Async alert controller interception
- **Key Feature**: Scene-based alert discovery and customization

### Windows

- **Native Control**: `ComboBox`
- **Approach**: XAML resource-based styling
- **Key Feature**: Dynamic style application via `ComboBox.Resources`

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022 (17.13+) or VS Code with .NET MAUI extension
- Platform workloads: Android SDK (API 21+), Xcode 15.0+ (macOS), Windows 10.0.17763.0+

## 📚 Related Resources

- [.NET MAUI Picker Control](https://learn.microsoft.com/dotnet/maui/user-interface/controls/picker)
- [Customizing Controls with Handlers](https://learn.microsoft.com/dotnet/maui/user-interface/handlers/customize)

## 🎯 What You'll Learn

**Core Concepts**:

- Extending MAUI controls with custom bindable properties
- Platform-specific handler implementation with partial classes
- Native control access and customization (`AlertDialog`, `UIPickerView`, `ComboBox`)
- Property mapper configuration with conditional compilation
- Color conversion between MAUI and platform-specific types

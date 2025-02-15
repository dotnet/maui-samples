---
name: .NET MAUI - Animations
description: "This sample demonstrates various animations you can do in .NET MAUI."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: animations
---

# Animations

This sample demonstrates how to use animations on any view in .NET MAUI.

### Basic Animations
The sample demonstrates fundamental MAUI animations through dedicated pages:
- **Scale**: Shows scaling animations with spring effects (`ScalePage.xaml`)
- **Rotate**: Demonstrates rotation on different axes (`RotatePage.xaml`) 
- **Fade**: Exhibits opacity transitions (`FadePage.xaml`)
- **Translate**: Showcases position movement (`TranslatePage.xaml`)

Each page implements basic animation methods like `ScaleTo()`, `RotateTo()`, `FadeTo()`, and `TranslateTo()` with consistent UI patterns and helpful tooltips.

### Easing Functions
The sample includes an interactive easing demonstration:
- **EasingsPage**: Shows different easing types
- **EasingEditorPage**: Provides visual representation of easing curves
- Implements various easing functions like `CubicIn`, `BounceOut`, and `SpringIn`

Each easing type can be visualized and tested through an interactive interface that shows how animations accelerate and decelerate.


### Custom Animations
Advanced animation system using the `Animation` class to create complex, synchronized animations with multiple properties and custom behaviors through the `Commit` method.
Demonstrates advanced animation techniques:
- **CustomAnimationPage**: Shows complex animation combinations
- Uses `Animation` class for custom effects
- Combines multiple animation types with various easing functions
- Implements color transitions and custom timing

The custom animations showcase how to build more complex, engaging user interactions by combining basic animations.

For more information about the sample see:

- [Basic Animations](https://docs.microsoft.com/dotnet/maui/user-interface/animation/basic)
- [Easing Functions](https://docs.microsoft.com/dotnet/maui/user-interface/animation/easing)
- [Custom Animations](https://docs.microsoft.com/dotnet/maui/user-interface/animation/custom)

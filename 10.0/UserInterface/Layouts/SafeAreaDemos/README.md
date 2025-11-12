---
name: .NET MAUI - SafeArea
description: This sample demonstrates how to use the .NET MAUI SafeAreaEdges property to control how content interacts with system UI elements like notches, status bars, and keyboards.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: userinterface-safearea
---

# SafeArea

This sample demonstrates how to use the .NET MAUI Multi-platform App UI (.NET MAUI) `SafeAreaEdges` property to control how content interacts with system UI elements like notches, status bars, navigation bars, and software keyboards.

The `SafeAreaEdges` property allows you to specify which edges of a view should respect the system's safe area insets, enabling both immersive edge-to-edge experiences and traditional safe layouts.

## Features

This sample includes the following examples:

### Basic Examples
- **Edge-to-Edge Content** - Demonstrates `SafeAreaEdges="None"` to create immersive full-screen experiences where content extends behind system bars
- **Respect All Safe Areas** - Shows `SafeAreaEdges="All"` to ensure content avoids all system UI elements including notches, status bars, and keyboards
- **Keyboard-Aware Layout** - Uses `SafeAreaEdges="SoftInput"` to create layouts that automatically adjust when the software keyboard appears

### Advanced Examples
- **Immersive Scrolling** - Demonstrates `SafeAreaEdges="Container"` for ScrollView to enable scrolling content that extends edge-to-edge
- **Per-Layout Control** - Shows how to use different `SafeAreaEdges` values on different layouts within the same page for fine-grained control
- **Best Practices** - Illustrates how to combine `SafeAreaEdges` with custom padding for optimal visual spacing

### NavigationPage Examples
- **Edge-to-Edge with NavigationPage** - Shows how to achieve edge-to-edge layouts with a semi-transparent navigation bar
- **Transparent Nav Bar** - Demonstrates creating immersive experiences with content visible behind the navigation bar

For more information about SafeArea in .NET MAUI, see [.NET MAUI SafeArea documentation](https://docs.microsoft.com/dotnet/maui/user-interface/layouts/safearea).

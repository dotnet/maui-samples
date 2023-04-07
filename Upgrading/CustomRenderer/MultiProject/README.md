---
name: Using Custom Renderers in .NET MAUI with multiple projects
description: "Sample for using custom renderers in .NET MAUI."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
- dotnet-core
urlFragment: custom-renderers
---

# Custom Renderers with Multiple Projects

When upgrading from Xamarin.Forms to .NET MAUI, custom renderers can be left in the platform projects. To register them, add a `MauiProgram.cs` file to each project and apply the platform specific code there. This sample demonstrates this approach.


---
name: .NET MAUI - Native embedding
description: This sample demonstrates how to embed a .NET MAUI UI in a .NET Android, .NET iOS, .NET Mac Catalyst, and WinUI app.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: platformintegration-nativeembedding
---

# Native embedding

Typically, a .NET Multi-platform App UI (.NET MAUI) app includes pages that contain layouts, such as `Grid`, and layouts that contain views, such as `Button`. Pages, layouts, and views all derive from `Element`. Native embedding enables any .NET MAUI controls that derive from `Element` to be consumed in .NET Android, .NET iOS, .NET Mac Catalyst, and WinUI native apps.

The process for consuming a .NET MAUI control in a native app is as follows:

1. Create extension methods to bootstrap your native embedded app.
1. Create a .NET MAUI single project that contains your .NET MAUI UI and any dependencies.
1. Create a native app and enable .NET MAUI support in it.
1. Initialize .NET MAUI by calling the `UseMauiEmbedding` extension method.
1. Create the .NET MAUI UI and convert it to the appropriate native type with the `ToPlatformEmbedding` extension method.

You must disable hot reload in Visual Studio to run this sample, or run the sample without debugging.

This sample contains multiple projects:

- NativeEmbeddingDemo - the .NET MAUI UIs for your native app.
- NativeEmbeddingDemo.Droid - the native app project for Android.
- NativeEmbeddingDemo.iOS - the native app project for iOS.
- NativeEmbeddingDemo.Library - helper code for native embedding scenarios.
- NativeEmbeddingDemo.MacCatalyst - the native app project for Mac Catalyst.
- NativeEmbeddingDemo.WinUI -the native app project for WinUI.
- TestHarnessApp - a test app that can be used to test .NET MAUI UI without having to launch the native apps. This enables you to iterate faster on your .NET MAUI UI by using XAML hot reload.

For more information about the sample see [Native embedding](https://learn.microsoft.com/dotnet/maui/platform-integration/native-embedding).

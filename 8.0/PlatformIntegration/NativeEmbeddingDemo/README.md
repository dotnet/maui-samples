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

Typically, a .NET Multi-platform App UI (.NET MAUI) app includes pages that contain layouts, such as <xref:Microsoft.Maui.Controls.Grid>, and layouts that contain views, such as <xref:Microsoft.Maui.Controls.Button>. Pages, layouts, and views all derive from <xref:Microsoft.Maui.Controls.Element>. Native embedding enables any .NET MAUI controls that derive from <xref:Microsoft.Maui.Controls.Element> to be consumed in .NET Android, .NET iOS, .NET Mac Catalyst, and WinUI native apps.

The process for consuming a .NET MAUI control in a native app is as follows:

1. Create extension methods to bootstrap your native embedded app.
1. Create a .NET MAUI single project that contains your .NET MAUI code and any dependencies.
1. Create a native app and enable .NET MAUI support in it.
1. Initialize .NET MAUI by calling the <xref:Microsoft.Maui.Embedding.AppHostBuilderExtensions.UseMauiEmbedding%2A> method.
1. Create an instance of the .NET MAUI control and convert it to the appropriate native type with the `ToPlatformEmbedded` extension method.

For more information about the sample see [Native embedding](https://learn.microsoft.com/dotnet/maui/platform-integration/native-embedding).

# .NET MAUI 11 Samples

Sample applications demonstrating .NET MAUI controls, platform features, and common app development scenarios for .NET 11.

## Getting started

- Install the [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0) and the .NET MAUI workload for your development platform. See [Install .NET MAUI](https://learn.microsoft.com/dotnet/maui/get-started/installation).
- For iOS and Mac Catalyst development, use a Mac with Xcode 27.2. Windows apps require Windows.
- Open a sample's solution or project and follow its README for any additional setup, such as service credentials, signing, or test devices.

[`global.json`](global.json) selects a stable .NET 11 SDK. It supports both a local `.dotnet/` installation and the system installation.

For example, to build the CollectionView sample for Mac Catalyst from this folder:

```bash
dotnet build UserInterface/Views/CollectionViewDemos/CollectionViewDemos/CollectionViewDemos.csproj \
  -f net11.0-maccatalyst -p:TargetFrameworks=net11.0-maccatalyst
```

The `TargetFrameworks` override limits restore and build to the selected platform. To launch the sample on the Mac, add `-t:Run`.

## Explore the samples

| Folder | Samples |
| ------ | ------- |
| [Apps](Apps/) | Complete apps, including Calculator, Weather '21, and Point of Sale. |
| [AI](AI/) | Chat clients with Microsoft.Extensions.AI and tool calling. |
| [Fundamentals](Fundamentals/) | Data binding, behaviors, templates, and localization. |
| [Navigation](Navigation/) | Navigation patterns, Shell, and route templates. |
| [PlatformIntegration](PlatformIntegration/) | Native platform features, including passkeys. |
| [UserInterface](UserInterface/) | Controls, layouts, gestures, styles, and maps. |
| [WebServices](WebServices/) | REST APIs, push notifications, and Aspire integration. |
| [XAML](XAML/) | XAML fundamentals and markup extensions. |

For list scenarios, see [CollectionViewDemos](UserInterface/Views/CollectionViewDemos/README.md#coming-from-listviewdemos), including equivalents of the earlier ListView demos.

## More information

- [.NET MAUI documentation](https://learn.microsoft.com/dotnet/maui/)
- [What's new in .NET MAUI 11](https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11?view=net-maui-11.0)
- [Centralized package versions](PACKAGE-VERSIONS.md)
- [Repository overview and sample highlights](../README.md)
- [.NET 10 samples](../10.0/)

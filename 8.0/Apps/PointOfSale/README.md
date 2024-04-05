---
name: .NET MAUI - Point of Sale
description: Point of Sale application that demonstrates UI techniques for customizing a sidebar menu, tab bars, radio buttons, and more. The project features a desktop/tablet UI and a mobile UI.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
- dotnet-core
urlFragment: apps-pointofsale
---

# Point of Sale (Demo App)

From [Focus on .NET Conf: MAUI](https://focus.dotnetconf.net).

This app demonstrates various techniques for building a desktop and mobile application that takes advantage of native platform features, and adapts to a wide range of device needs all from a single codebase. This is NOT a complete application.

<img width="1100" alt="food-mac" src="https://user-images.githubusercontent.com/41873/183739194-0e19cb9c-71aa-490b-9e80-25ea1dc40ce0.png">

![food-mobile-2](https://user-images.githubusercontent.com/41873/183740348-7f55d10d-8f79-4ee0-a71e-64b317cbd64f.png)

## Local Testing

If you want to test the full functionality of this app locally for iOS, make sure to rename the `Platforms/iOS/EntitlementsSample.plist` file to ``Platforms/iOS/Entitlements.plist`.

By doing so, you will need a matching provisioning profile in your Apple Developer accound in order for everything to work correctly.

### Credits

* Original design: https://www.uplabs.com/posts/foodos-food-point-of-sale
* Various graphics: https://mockup.graphics

* .NET MAUI: http://docs.microsoft.com/dotnet/maui
* CommunityToolkit.Mvvm: https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction
* CommunityToolkit.Maui: https://github.com/CommunityToolkit/Maui
* SkiaSharp & Lottie: https://mono.github.io/SkiaSharp.Extended/api/ui-maui/#sklottieview
* MicroCharts: https://github.com/microcharts-dotnet/Microcharts
* Ril.BlazorSignatureCanvas: https://github.com/ResourceWare/Ril.BlazorSignatureCanvas
# .NET MAUI 10: New Features Guide

[Back to top](#net-maui-10-new-features-guide)

---

## Table of Contents

1. [Overview](#overview)
2. [Core Features](#core-features)
   - [XAML Source Generation](#xaml-source-generation)
   - [Global XML Namespaces](#global-xml-namespaces)
   - [OpenTelemetry Diagnostics](#opentelemetry-diagnostics)
3. [Control Enhancements](#control-enhancements)
   - [WebView / BlazorWebView Improvements](#webview--blazorwebview-improvements)
   - [MediaPicker Enhancements](#mediapicker-enhancements)
   - [UI Control Updates](#ui-control-updates)
   - [Essentials API Additions](#essentials-api-additions)
4. [Platform-Specific Features](#platform-specific-features)
   - [iOS and Mac Catalyst](#ios-and-mac-catalyst)
   - [Android](#android)
5. [.NET Aspire Integration](#net-aspire-integration)

---

## Overview

.NET MAUI 10 introduces several new features focused on performance, developer productivity, and observability. This guide covers the new capabilities available in .NET MAUI 10 that can enhance your applications.

**Key Highlights:**
- XAML Source Generation for 2-3x faster startup
- Global XML Namespaces to reduce XAML boilerplate
- OpenTelemetry integration for diagnostics
- .NET Aspire support for distributed applications
- Enhanced WebView capabilities
- Improved media picker with compression
- Platform-specific UI enhancements

[Back to top](#net-maui-10-new-features-guide)

---

## Core Features

### XAML Source Generation

XAML Source Generation generates strongly-typed C# code from your XAML files at build time instead of parsing XML at runtime.

**Benefits:**
- **2-3x faster startup** on iOS and Android
- **Lower memory usage** by eliminating reflection
- **Better trimming** for NativeAOT on iOS
- **Compile-time XAML validation** (catch errors before runtime)
- **Full NativeAOT compatibility**
- **Hot Reload fully supported**

**How to Enable:**

Add this to your .csproj file:

```xml
<PropertyGroup>
  <MauiXamlInflator>SourceGen</MauiXamlInflator>
</PropertyGroup>
```

**Trade-offs:**
- Slightly longer initial build times
- No changes to your XAML code required

Your existing XAML will continue to work without changes. Source generation is recommended for all projects but not required.

[Back to top](#net-maui-10-new-features-guide)

---

### Global XML Namespaces

Eliminate XAML namespace boilerplate by registering namespaces globally.

**Before (.NET 9):**

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:models="clr-namespace:MyApp.Models"
             xmlns:controls="clr-namespace:MyApp.Controls"
             xmlns:converters="clr-namespace:MyApp.Converters"
             xmlns:local="clr-namespace:MyApp.Views"
             x:Class="MyApp.MainPage">
    <controls:TagView x:DataType="models:Tag" />
    <converters:BoolToColorConverter />
</ContentPage>
```

**After (.NET 10):**

Create **GlobalXmlns.cs** at project root:

```csharp
[assembly: XmlnsDefinition(
    "http://schemas.microsoft.com/dotnet/maui/global",
    "MyApp.Views")]
[assembly: XmlnsDefinition(
    "http://schemas.microsoft.com/dotnet/maui/global",
    "MyApp.Controls")]
[assembly: XmlnsDefinition(
    "http://schemas.microsoft.com/dotnet/maui/global",
    "MyApp.Converters")]
[assembly: XmlnsDefinition(
    "http://schemas.microsoft.com/dotnet/maui/global",
    "MyApp.Models")]

// Optional: default prefixes for disambiguation
using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

[assembly: XmlnsPrefix("MyApp.Controls", "controls")]
[assembly: XmlnsPrefix("MyApp.Converters", "converters")]
```

**New XAML:**

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/maui/global"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyApp.MainPage">
    <controls:TagView x:DataType="Tag" />
    <converters:BoolToColorConverter />
</ContentPage>
```

**Benefits:**
- Reduce XAML file sizes significantly
- Fewer namespace declarations to maintain
- Easier refactoring when reorganizing code
- Better onboarding for new team members

[Back to top](#net-maui-10-new-features-guide)

---

### OpenTelemetry Diagnostics

.NET MAUI 10 includes OpenTelemetry instrumentation for layout performance metrics.

**Available Metrics:**

| Metric | Type | Unit | Description |
|--------|------|------|-------------|
| `maui.layout.measure_count` | Counter | integer | Total measure operations |
| `maui.layout.measure_duration` | Histogram | nanoseconds | Measure operation latency |
| `maui.layout.arrange_count` | Counter | integer | Total arrange operations |
| `maui.layout.arrange_duration` | Histogram | nanoseconds | Arrange operation latency |

**Integration:**

```csharp
// In MauiProgram.cs
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeterListener()
            .AddMauiInstrumentation();  // Collects MAUI metrics
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddSource("Microsoft.Maui")
            .AddOtlpExporter();  // Send to observability backend
    });
```

**Use Cases:**
- Identify layout performance bottlenecks
- Monitor scroll performance in large lists
- Track animation frame rates
- Debug UI jank issues

[Back to top](#net-maui-10-new-features-guide)

---

## Control Enhancements

### WebView / BlazorWebView Improvements

#### Request Interception

Intercept and modify web requests before the WebView processes them:

```csharp
webView.WebResourceRequested += (s, e) =>
{
    // Scenario 1: Inject auth headers
    if (e.Uri.Host == "api.example.com")
    {
        e.RequestHeaders["Authorization"] = $"Bearer {GetUserToken()}";
    }

    // Scenario 2: Serve local content for offline support
    if (e.Uri.ToString().EndsWith("data.json"))
    {
        e.Handled = true;
        e.SetResponse(200, "OK", "application/json", GetLocalDataStream());
    }

    // Scenario 3: Block third-party trackers
    if (e.Uri.Host.Contains("analytics-tracker.com"))
    {
        e.Handled = true;
        e.SetResponse(204, "No Content", "text/plain", null);
    }
};
```

**Important:**
- Handler MUST be synchronous
- If `Handled = true`, MUST provide response via `SetResponse()`
- Do NOT perform long-running operations; WebView will freeze

#### New Events

```csharp
webView.WebViewInitializing += (s, e) =>
{
    // Before native WebView created
    // Configure platform-specific settings
};

webView.WebViewInitialized += (s, e) =>
{
    // After native WebView created
    // Access platform-specific instance
    #if IOS
    var nativeWebView = (WKWebView)e.NativeWebView;
    nativeWebView.ScrollView.ScrollEnabled = false;
    #endif
};
```

#### JavaScript Exception Handling

Exceptions thrown in JavaScript are automatically re-thrown as .NET exceptions:

```csharp
try
{
    await webView.InvokeJavaScriptAsync("doSomething", "arg1");
}
catch (Exception ex)
{
    // ex.Message contains JavaScript error
}
```

#### Android Fullscreen Video

MAUI 10 properly handles fullscreen video on Android via WebChromeClient overrides. Previously, tapping fullscreen would fail.

[Back to top](#net-maui-10-new-features-guide)

---

### MediaPicker Enhancements

#### Multiple File Selection

```csharp
var photos = await MediaPicker.PickMultipleAsync(new MediaPickerOptions
{
    Title = "Select up to 10 photos"
});

foreach (var photo in photos)
{
    var stream = await photo.OpenReadAsync();
    await UploadPhotoAsync(stream);
}
```

#### Automatic Image Compression

```csharp
var result = await MediaPicker.PickMultipleAsync(new MediaPickerOptions
{
    MaximumWidth = 1024,    // Downscale to max 1024px
    MaximumHeight = 768,    // Downscale to max 768px
});
```

#### EXIF Auto-Rotation

```csharp
var photo = await MediaPicker.PickPhotoAsync();
// Photo is automatically rotated based on device orientation metadata
// EXIF data preserved for camera/timestamp info
```

[Back to top](#net-maui-10-new-features-guide)

---

### UI Control Updates

#### SearchBar

```xaml
<SearchBar Placeholder="Search..."
           SearchIconColor="Blue"
           ReturnType="Search" />
```

New properties:
- `SearchIconColor`: Color of the search magnifying glass icon
- `ReturnType`: Appearance of return/search button (Search, Go, Done, Send, Next, Previous)

#### Switch

```xaml
<Switch OffColor="Red"
        OnColor="Green" />
```

Control color when switch is OFF (previously only OnColor available).

#### RefreshView

```xaml
<RefreshView IsRefreshEnabled="false">
    <!-- Content remains interactive even though refresh is disabled -->
    <StackLayout>
        <Entry Placeholder="Username" />
        <Entry Placeholder="Password" />
        <Button Text="Login" />
    </StackLayout>
</RefreshView>
```

Distinct from `IsEnabled`. Allows disabling swipe-to-refresh while keeping content interactive.

#### Picker

```csharp
// Open picker programmatically
picker.Open();

// Close picker programmatically
picker.Close();
```

[Back to top](#net-maui-10-new-features-guide)

---

### Essentials API Additions

#### Text-to-Speech Rate Control

```csharp
var options = new SpeechOptions()
{
    Pitch = 1.0f,      // Normal pitch
    Volume = 0.8f,     // 80% volume
    Rate = 0.5f,       // 50% speed (slower)
    Locale = locale
};

await TextToSpeech.Default.SpeakAsync("Hello", options);
```

#### WebAuthenticator CancellationToken Support

```csharp
var cts = new CancellationTokenSource(timeout: TimeSpan.FromMinutes(5));

try
{
    var result = await WebAuthenticator.AuthenticateAsync(
        new WebAuthenticatorOptions
        {
            Url = new Uri("https://auth.example.com/authorize"),
            CallbackUrl = new Uri("myapp://auth-callback"),
        },
        cancellationToken: cts.Token);
}
catch (OperationCanceledException)
{
    // User took too long or cancelled
}
```

#### Vibration / HapticFeedback IsSupported Check

```csharp
if (HapticFeedback.Default.IsSupported)
{
    HapticFeedback.Default.Perform(HapticFeedbackType.Click);
}

if (Vibration.Default.IsSupported)
{
    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
}
```

#### Geolocation IsEnabled Property

```csharp
if (!Geolocation.IsEnabled)
{
    await DisplayAlert("Location", "Please enable location services", "OK");
    return;
}

var location = await Geolocation.GetLocationAsync();
```

#### TabbedPageManager / StackNavigationManager Now Public

```csharp
#if ANDROID
// Customize tab navigation behavior on Android
var manager = TabbedPage.GetManager(this) as TabbedPageManager;
manager.UpdateCurrentPage(newIndex);
#endif
```

[Back to top](#net-maui-10-new-features-guide)

---

## Platform-Specific Features

### iOS and Mac Catalyst

#### Modal Popovers

Display modal pages as popovers anchored to a view:

```csharp
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

public partial class PopoverPage : ContentPage
{
    public PopoverPage(View sourceView, Rectangle sourceRect)
    {
        InitializeComponent();
        On<iOS>()
            .SetModalPopoverView(sourceView)
            .SetModalPopoverRect(sourceRect)
            .SetModalPresentationStyle(UIModalPresentationStyle.Popover);
    }
}

// Usage
var popover = new PopoverPage(originButton, Rectangle.Empty);
await Navigation.PushModalAsync(popover);
```

**Use cases:**
- Context menus
- Filter options
- Quick settings from toolbar button
- Non-intrusive editing workflows

#### Secondary Toolbar Items

Modern iOS 13+ pattern with secondary items in overflow menu:

```xaml
<ContentPage.ToolbarItems>
    <!-- Primary items appear directly -->
    <ToolbarItem Text="Save" Order="Primary" Priority="0" />
    <ToolbarItem Text="Share" Order="Primary" Priority="1" />

    <!-- Secondary items appear in overflow menu -->
    <ToolbarItem Text="Delete" Order="Secondary" Priority="0" />
    <ToolbarItem Text="Export" Order="Secondary" Priority="1" />
    <ToolbarItem Text="Settings" Order="Secondary" Priority="2" />
</ContentPage.ToolbarItems>
```

#### Enhanced SafeArea Control

Granular control over safe area behavior:

```csharp
public enum SafeAreaRegions
{
    None = 0,          // Edge-to-edge (ignore safe areas)
    SoftInput = 1,     // Pad for keyboard/soft input only
    Container = 2,     // Flow under keyboard, stay out of bars/notch
    Default = 4,       // Platform default
    All = int.MaxValue // Obey all safe area insets
}
```

Usage:

```xaml
<ContentPage SafeAreaEdges="Container">
    <ScrollView SafeAreaEdges="None">
        <!-- Edge-to-edge content inside scrollable area -->
    </ScrollView>
</ContentPage>
```

#### CollectionView / CarouselView Optimized Handlers

Improved handlers are now default in MAUI 10:

Platform | .NET 9 | .NET 10 | Benefit
---|---|---|---
iOS | Optional | **Default** | Better performance, memory
Mac Catalyst | Optional | **Default** | Better performance, memory

[Back to top](#net-maui-10-new-features-guide)

---

### Android

#### Design-Time Build Performance

MAUI 10 parses .aar files directly instead of running aapt2:
- Before: Multi-second delays for design-time build
- After: Sub-second design-time build
- Impact: IntelliSense feels instant

#### APK/AAB Creation Speed

Command-line builds now use `System.IO.Compression` instead of `libzipsharp`:
- Faster APK/AAB generation

#### dotnet run Support

Deploy directly to devices without Visual Studio:

```bash
# Run on attached physical device
dotnet run -p:AdbTarget=-d

# Run on emulator
dotnet run -p:AdbTarget=-e

# Run on specific device
dotnet run -p:AdbTarget="-s emulator-5554"
```

#### JDK 21 Support

Build with modern Java tooling:

```bash
java -version
# openjdk 21.x.x or higher
```

#### AndroidMavenLibrary ArtifactFilename

Bind Java libraries from Maven with custom filenames:

```xml
<ItemGroup>
    <AndroidMavenLibrary Include="com.facebook.react:react-android"
                          Version="0.76.0"
                          ArtifactFilename="react-android-0.76.0-release.aar" />
</ItemGroup>
```

[Back to top](#net-maui-10-new-features-guide)

---

## .NET Aspire Integration

.NET Aspire provides service discovery, telemetry, and orchestration for distributed applications including MAUI apps.

### Project Structure

Three key projects required:

1. **MAUI Service Defaults Project** - Shared configuration
2. **App Host Project** - Orchestrator for all services
3. **MAUI App Project** - Your mobile application

### Setup

#### 1. Create Service Defaults Project

```bash
dotnet new maui-aspire-servicedefaults -n YourApp.MauiServiceDefaults
dotnet add YourMauiApp.csproj reference YourApp.MauiServiceDefaults/YourApp.MauiServiceDefaults.csproj
```

#### 2. Create App Host Project

```bash
dotnet new aspire-apphost -n YourApp.AppHost
dotnet add YourApp.AppHost.csproj reference YourMauiApp/YourMauiApp.csproj
dotnet add YourApp.AppHost.csproj reference YourWebService/YourWebService.csproj
```

#### 3. Configure App Host

In `YourApp.AppHost/Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Register your web service
var weatherApi = builder.AddProject("webapi", @"../YourWebService/YourWebService.csproj");

// Create a public dev tunnel for iOS and Android
var publicDevTunnel = builder.AddDevTunnel("devtunnel-public")
    .WithAnonymousAccess()
    .WithReference(weatherApi.GetEndpoint("https"));

// Register your MAUI app
var mauiapp = builder.AddMauiProject("mauiapp", @"../YourMauiApp/YourMauiApp.csproj");

// Add Windows device (uses localhost directly)
mauiapp.AddWindowsDevice()
    .WithReference(weatherApi);

// Add iOS simulator with Dev Tunnel
mauiapp.AddiOSSimulator()
    .WithOtlpDevTunnel()  // Required for OpenTelemetry data collection
    .WithReference(weatherApi, publicDevTunnel);

// Add Android emulator with Dev Tunnel
mauiapp.AddAndroidEmulator()
    .WithOtlpDevTunnel()  // Required for OpenTelemetry data collection
    .WithReference(weatherApi, publicDevTunnel);

builder.Build().Run();
```

#### 4. Configure MAUI App

In `MauiProgram.cs`:

```csharp
var builder = MauiApp.CreateBuilder();

builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
    });

// Add service defaults from MAUI Service Defaults project
builder.Services.AddServiceDefaults();

// Configure HTTP client with service discovery
// Service name matches AppHost identifier
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new Uri("https+http://webapi");
});

return builder.Build();
```

**Note:** `https+http://` syntax enables both HTTPS and HTTP protocols, with preference for HTTPS.

### Features

**Service Discovery:**
- Automatic service endpoint resolution
- No hardcoded URLs or IP addresses
- Works across all platforms

**Dev Tunnels:**
- Secure connectivity for iOS and Android
- No need for 10.0.2.2 configuration
- Automatic HTTPS setup

**Observability:**
- Unified dashboard for all services
- Distributed tracing across services
- Real-time logs and metrics
- OpenTelemetry integration

**Platform Support:**
- Windows: Direct localhost connectivity
- Mac Catalyst: Direct localhost connectivity
- iOS: Via Dev Tunnels
- Android: Via Dev Tunnels

### Running Your Application

1. Set the **App Host project as the startup project**
2. Run the solution
3. **Aspire dashboard opens** automatically
4. Your **MAUI app launches** on the selected platform
5. **Automatic service connectivity** works out of the box

### Sample Application

Official example: [AspireWithMaui sample](https://github.com/dotnet/aspire/tree/main/playground/AspireWithMaui) in the Aspire repository

[Back to top](#net-maui-10-new-features-guide)

---

**Document Version:** 1.0
**Last Updated:** November 2025
**Applies To:** .NET MAUI 10.0.100 and later

[Back to top](#net-maui-10-new-features-guide)
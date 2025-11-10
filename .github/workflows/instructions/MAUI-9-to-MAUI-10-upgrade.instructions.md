# .NET MAUI 10: Comprehensive Feature Deep Dive

## Executive Summary

.NET MAUI 10 represents a **maturation milestone** for cross-platform .NET development, focusing on production readiness, observability, and developer quality-of-life. Released November 2025, it delivers first-class .NET Aspire integration for end-to-end telemetry, XAML source generation for improved startup performance, simplified namespace boilerplate, and significant platform enhancements for Android, iOS, Mac Catalyst, and Windows. The release prioritizes stability and developer ergonomics, with emphasis on supporting the latest OS versions (iOS 18.2, Android 16/API 36, macOS 15.2).

---

## Part I: Core Ecosystem Enhancements

### 1. .NET Aspire Integration: Unified Cloud-Native Mobile

> "Aspire provides a consistent, opinionated set of tools and patterns for building and running distributed applications. It's designed to improve the experience of building cloud-native applications by providing: Orchestration, Components, Tooling, and Service discovery."

#### Project Structure: Three Key Projects

The official documentation defines the required project structure:

#### 1. MAUI Service Defaults Project

Contains shared configuration for your MAUI app:

```bash
dotnet new maui-aspire-servicedefaults -n YourApp.MauiServiceDefaults
```

Add reference to it from your MAUI app:

```bash
dotnet add YourMauiApp.csproj reference YourApp.MauiServiceDefaults/YourApp.MauiServiceDefaults.csproj
```

**What it includes:**
- Service discovery configuration
- Default resilience patterns
- Telemetry setup
- Platform-specific networking configuration

#### 2. App Host Project (Orchestrator)

Orchestrates all application services:

```bash
dotnet new aspire-apphost -n YourApp.AppHost
```

Add references to MAUI app and services:

```bash
dotnet add YourApp.AppHost.csproj reference YourMauiApp/YourMauiApp.csproj
dotnet add YourApp.AppHost.csproj reference YourWebService/YourWebService.csproj
```

#### 3. MAUI App Project

Your actual mobile application, configured with service discovery.

---

#### Configure the App Host

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

// Add Mac Catalyst device (uses localhost directly)
mauiapp.AddMacCatalystDevice()
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

**Key Points:**

- **Service names** must match between AppHost and client configuration
- **Dev Tunnels** created separately for each platform
- **WithOtlpDevTunnel()** required for iOS/Android telemetry to reach Aspire dashboard
- **Multiple devices supported:** Add Windows, Mac Catalyst, iOS, and Android simultaneously
- Deploy and test on multiple targets from the same App Host

---

#### Configure Your MAUI App

In `MauiProgram.cs`:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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
    }
}
```

**Special Scheme:** `https+http://` syntax enables both HTTPS and HTTP protocols, with preference for HTTPS. The service name (`webapi`) must match the name in the App Host.

---

#### Create a Service Client

```csharp
public class WeatherApiClient
{
    private readonly HttpClient _httpClient;

    public WeatherApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecast[]?> GetWeatherForecastAsync(
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<WeatherForecast[]>(
            "/weatherforecast",
            cancellationToken);
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

---

#### Use the Client in Your App

Inject into pages or view models:

```csharp
public partial class MainPage : ContentPage
{
    private readonly WeatherApiClient _weatherClient;

    public MainPage(WeatherApiClient weatherClient)
    {
        _weatherClient = weatherClient;
        InitializeComponent();
    }

    private async void OnGetWeatherClicked(object sender, EventArgs e)
    {
        try
        {
            var forecasts = await _weatherClient.GetWeatherForecastAsync();
            
            if (forecasts != null)
            {
                ResultLabel.Text = $"Retrieved {forecasts.Length} forecasts";
            }
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"Error: {ex.Message}";
        }
    }
}
```

---

#### Platform-Specific Considerations

#### iOS and Mac Catalyst

- Service discovery configuration automatically provides correct URLs
- Dev Tunnels automatically configured for iOS Simulator and physical devices
- Aspire dashboard shows active connections and logs
- No Apple Transport Security (ATS) configuration needed

#### Android

Aspire integration handles platform-specific requirements automatically:

**You no longer need to:**
- Configure the special `10.0.2.2` address
- Set up network security configuration files
- Enable clear-text traffic for local development

**Instead:**
- Dev Tunnels provide secure, reliable connectivity between Android emulator and local services

#### Dev Tunnels Integration

Dev Tunnels provide a secure way to expose local web services to mobile devices and emulators:

**Aspire automatically:**
- Creates and manages Dev Tunnels for your services
- Configures your MAUI app to use tunnel URLs
- Handles authentication and connection management
- Eliminates complex network configuration

##### OpenTelemetry Data Collection (Critical)

**For iOS and Android:**

```csharp
mauiapp.AddiOSSimulator()
    .WithOtlpDevTunnel()  // Required for OpenTelemetry data collection
    .WithReference(weatherApi, publicDevTunnel);

mauiapp.AddAndroidEmulator()
    .WithOtlpDevTunnel()  // Required for OpenTelemetry data collection
    .WithReference(weatherApi, publicDevTunnel);
```

The `WithOtlpDevTunnel()` method creates a Dev Tunnel specifically for OpenTelemetry protocol (OTLP) traffic, allowing telemetry data from iOS and Android apps to reach the Aspire dashboard on the development machine.

**Without this:**
- iOS and Android apps won't send telemetry data to the Aspire dashboard
- Monitoring and debugging capabilities unavailable for mobile platforms
- Traces and metrics missing from observability dashboard

#### Windows

Local service connectivity works directly through localhost without requiring additional configuration. Aspire provides a consistent API across platforms, but Windows implementation is straightforward.

---

#### Running Your Application

**To run your MAUI app with Aspire integration:**

1. Set the **App Host project as the startup project**
2. Run the solution
3. The **Aspire dashboard opens** automatically, showing all registered services
4. Your **MAUI app launches** on the selected platform
5. **Automatic service connectivity** through App Host orchestration

**When running through App Host:**
- All services start automatically
- Service discovery is configured
- Dashboard provides real-time monitoring
- Logs from all services available in one place

**Selecting Platform Target:**
1. Use target framework dropdown to select platform (Android, iOS, Windows, etc.)
2. App Host launches MAUI app on selected platform
3. Service connectivity works automatically on all platforms

---

#### Monitoring and Debugging via Aspire Dashboard

**Available Tools:**

- **Resource view:** See all running services and their status
- **Logs:** View combined logs from all services in one place
- **Traces:** Distributed tracing across services
- **Metrics:** Monitor performance and resource usage

---

#### Troubleshooting Aspire Integration

#### Missing Metrics or Traces from iOS/Android Apps

**Problem:** Not seeing telemetry data in Aspire dashboard

**Solution:** Verify `WithOtlpDevTunnel()` in device configurations:

```csharp
mauiapp.AddiOSSimulator()
    .WithOtlpDevTunnel()  // Required!
    .WithReference(weatherApi, publicDevTunnel);

mauiapp.AddAndroidEmulator()
    .WithOtlpDevTunnel()  // Required!
    .WithReference(weatherApi, publicDevTunnel);
```

Without this, telemetry cannot reach the Aspire dashboard from mobile devices.

#### Service Discovery Not Working

**Checklist:**

1. Verify `AddServiceDefaults()` called in `MauiProgram.cs`
2. Service name in HTTP client configuration matches AppHost identifier
3. Using `https+http://` scheme in service URL
4. For iOS/Android: Dev Tunnels configured correctly in AppHost

#### Dev Tunnels Connection Issues

1. Dev Tunnel configured with anonymous access: `.WithAnonymousAccess()`
2. Device configuration includes Dev Tunnel reference: `.WithReference(weatherApi, publicDevTunnel)`
3. Firewall/network security not blocking tunnel connections
4. Try restarting App Host to recreate tunnels

#### App Host Won't Start

1. Verify all project paths in `Program.cs` are correct (use relative paths)
2. All referenced projects build successfully independently
3. .NET 10 SDK and Aspire properly installed
4. Review App Host console output for specific errors

#### MAUI App Can't Find Service Defaults

1. Verify reference to MAUI Service Defaults project exists
2. Service Defaults project builds successfully
3. `AddServiceDefaults()` called before configuring HTTP clients

---

#### Best Practices for Aspire with MAUI

- **Use typed clients:** Create strongly-typed HTTP clients for each service
- **Handle errors gracefully:** Network operations can fail; implement proper error handling and retry logic
- **Leverage the dashboard:** Use Aspire dashboard for debugging and monitoring during development
- **Test on all platforms:** While integration handles platform differences, always test on target platforms
- **Follow service defaults:** Service Defaults project provides recommended patterns for resilience and telemetry

---

#### Sample Application

Official example: [AspireWithMaui sample](https://github.com/dotnet/aspire/tree/main/playground/AspireWithMaui) in the Aspire repository

**Demonstrates:**
- Complete project structure
- Service registration and discovery
- Platform-specific considerations
- Error handling and resilience patterns

---

### 2. XAML Source Generation: Build-Time IL, Runtime Wins

**Evolution:** MAUI has supported XAML source generation since preview, but RC1 simplified enablement significantly. Previous preview enablement (assembly attributes, define constants) is now obsolete.

**RC1+ Enablement (Current Standard):**

```xml
<PropertyGroup>
  <MauiXamlInflator>SourceGen</MauiXamlInflator>
</PropertyGroup>
```

That's it.

#### What the Generated Code Actually Does

The actual MAUI source generator produces:

```csharp
// Generated file: PageName.xaml.sg.cs
[Generated]
public partial class MainPage : ContentPage
{
    private void InitializeComponent()
    {
        // Pre-instantiate and register all XAML extensions
        var eventToCommandBehavior1 = new global::CommunityToolkit.Maui.Behaviors.EventToCommandBehavior();
        var referenceExtension2 = new global::Microsoft.Maui.Controls.Xaml.ReferenceExtension();
        var bindingExtension5 = new global::Microsoft.Maui.Controls.Xaml.BindingExtension();
        
        // Register source location information for design-time support and diagnostics
        global::Microsoft.Maui.VisualDiagnostics.RegisterSourceInfo(
            eventToCommandBehavior1!, 
            new global::System.Uri("@Pages\MainPage.xaml;assembly=YourApp", global::System.UriKind.Relative), 
            19, 10);
        
        // Additional registrations for each extension and control...
        // Registers line/column numbers for hot reload and error reporting
    }
}
```

**How It Works Under the Hood:**

Traditional XAML (runtime):
1. XAML file parsed as XML at app startup
2. Type names resolved via reflection (expensive on startup thread)
3. Controls instantiated via `Activator.CreateInstance()` (reflection)
4. Data binding expressions compiled on-the-fly
5. Hot reload compatible but startup-slow

Source-Generated XAML (build-time):
1. Build process runs source generator on .xaml files
2. Generates strongly-typed C# code (decorated with [Generated] attribute)
3. Complex control hierarchies become direct `new Label() { ... }` chains
4. Data binding expressions pre-compiled as lambda expressions
5. InitializeComponent() becomes a simple method with no reflection

**Performance Characteristics:**

Provides XAML inflation timing improvements.

Key observations:
- **Debug Build Startup:** Noticeably faster on both iOS and Android
- **Memory Footprint:** Lower peak memory due to elimination of reflection metadata
- **Build Time Impact:** Initial builds slightly slower, but incremental builds comparable
- **AOT Compatibility:** Enables full NativeAOT on iOS with better trimming

**Dev Experience Trade-offs:**

- Faster debug builds (scrolling feels smoother during iteration)
- Debug/Release builds behave identically (eliminates surprising prod bugs)
- Full NativeAOT compatibility (smaller iOS apps)
- Better IntelliSense (source-generated types are strongly-typed)
- Initial build time increase
- If you modify generated code, changes are lost on next build (don't do this)

**Enterprise Recommendation:** Migrate existing projects gradually during feature sprints (no urgent deadline since both modes work; source gen is just better).

---

### 3. Global and Implicit XML Namespaces: XAML Boilerplate Eliminated

**The Old Problem:**

Every XAML file began with copy-paste namespace drudgery:

```xaml
<!-- .NET 8 style - every file looks like this -->
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

For a 100-file MAUI app, this represents more than 1,000 lines of ceremonial XML.

**MAUI 10 Solution: Global Namespace Registration**

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

**New XAML - Approach 1: Global Namespace with Explicit Prefixes (Safest)**

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/maui/global"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyApp.MainPage">
    <controls:TagView x:DataType="Tag" />
    <converters:BoolToColorConverter />
</ContentPage>
```

Changes:
- Single xmlns pointing to global namespace
- No need to declare xmlns:models, xmlns:controls, etc.
- Prefixes still explicit (good for large teams with naming collisions)
- **Backward compatible:** old explicit xmlns still work

**New XAML - Approach 2: Implicit Global Namespace (Maximum Simplification)**

Requires opt-in feature switch. In RC1+, not a separate compilation mode:

```xaml
<ContentPage x:Class="MyApp.MainPage">
    <TagView x:DataType="Tag" />
    <BoolToColorConverter />
</ContentPage>
```

Changes:
- Zero xmlns declarations
- Types resolved from global namespace automatically
- Compiler injects implicit xmlns at parse time
- **Risk:** Collisions if two namespaces export same type name

**Adoption Strategy:**

- **Small teams / new projects:** Use Approach 2 (maximum simplification)
- **Large teams / existing codebases:** Use Approach 1 (global + explicit prefixes for clarity)
- **Mixed codebases:** Start with Approach 1, migrate to Approach 2 gradually
- **NuGet library discovery:** Can register third-party library types in GlobalXmlns.cs too

**Developer Productivity Impact:**

- **Time saved:** No namespace hunting/copy-pasting
- **Error reduction:** Substantially fewer typo-related namespace issues
- **Onboarding:** New team members don't need to memorize namespace patterns
- **Refactoring:** Reorganizing namespaces affects one file (GlobalXmlns.cs) instead of dozens of XAML files

---

## Part II: Diagnostics and Performance Observability

### Diagnostics Infrastructure: Layout Performance Metrics

**Why This Matters:**

Mobile UI performance is rarely about crashes; it's about subtle degradations. Jank on scroll, delayed button response, laggy animations accumulate as negative user experience. Traditional debugging with breakpoints masks real performance issues (breakpoints slow execution dramatically). MAUI 10 ships with enterprise-grade diagnostics to instrument the problem.

**What Gets Instrumented:**

Two fundamental operations drive layout performance:

1. **Measure (IView.Measure()):** Framework calculates size of elements
2. **Arrange (IView.Arrange()):** Framework positions elements in parent

If these operations are called excessively or slowly, the main thread saturates, and animations skip frames.

**Available Metrics:**

| Metric | Type | Unit | Semantics |
|--------|------|------|-----------|
| `maui.layout.measure_count` | Counter | integer | Total measure operations (cumulative) |
| `maui.layout.measure_duration` | Histogram | nanoseconds | Latency distribution per measure call |
| `maui.layout.arrange_count` | Counter | integer | Total arrange operations (cumulative) |
| `maui.layout.arrange_duration` | Histogram | nanoseconds | Latency distribution per arrange call |

**Integration with OpenTelemetry:**

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

**Real-World Debugging Scenario with Aspire Dashboard:**

Production complaint: "ListView with 500 items stutters when scrolling."

1. Configure your MAUI app to export telemetry to Aspire Dashboard OTLP endpoint (via AddOtlpExporter)
2. Run Aspire Dashboard in standalone mode or via your AppHost
3. Reproduce the issue in staging
4. Open Aspire Dashboard → Metrics tab → View `maui.layout.arrange_duration` histogram by resource
5. Find: ListView item template arrange operations exceed frame budget (frames at 60Hz have a 16ms time window)
6. Examine which control type produces slow arrange times (Aspire shows breakdowns by span attributes)
7. Optimize template in code (remove unnecessary nested layouts)
8. Re-run with Aspire Dashboard open
9. Watch metrics update in real-time: arrange duration drops as you fix each issue
10. Metrics confirm: stutter eliminated

**Aspire Dashboard Advantages for Diagnostics:**

- **No external services:** Dashboard runs locally (standalone Docker container or built-in to AppHost)
- **Real-time visualization:** Metrics, traces, and logs update live as you interact with your app
- **Structured data:** Click into any metric to see the context, resource details, and associated spans
- **Distributed tracing:** See the complete call chain (client layout operation → backend response if data-bound)
- **Zero production data:** All telemetry stays on your development machine during debugging

---

## Part III: Control and Animation Enhancements

### Animation: Async-First Paradigm

**Deprecation Status:** All synchronous animation methods deprecated in favor of async counterparts.

**Deprecated Methods (Full List):**
- `FadeTo()` → `FadeToAsync()`
- `ScaleTo()` → `ScaleToAsync()`
- `TranslateTo()` → `TranslateToAsync()`
- `RotateTo()` → `RotateToAsync()`
- `RotateXTo()` → `RotateXToAsync()`
- `RotateYTo()` → `RotateYToAsync()`
- `ScaleXTo()` → `ScaleXToAsync()`
- `ScaleYTo()` → `ScaleYToAsync()`
- `LayoutTo()` → `LayoutToAsync()`
- `RelRotateTo()` → `RelRotateToAsync()`
- `RelScaleTo()` → `RelScaleToAsync()`

**Why Async Is Better:**

Old pattern (fire-and-forget):
```csharp
view.FadeTo(0, 500);  // Returns void, no guarantee when done
// This code runs immediately, animation happens in background
await DisplayAlert("Done", "Animation might not be finished!", "OK");
```

New pattern (sequential, composable):
```csharp
await view.FadeToAsync(0, 500);  // Guaranteed finished
await DisplayAlert("Done", "Animation definitely finished!", "OK");
```

**Cancellation Support:**

```csharp
var cts = new CancellationTokenSource();

// Start animation (optional cancellation token)
var task = view.TranslateToAsync(500, 500, duration: 5000, easing: Easing.CubicInOut);

// User taps cancel button
if (cancelButtonTapped)
    cts.Cancel();

try
{
    await task;
}
catch (OperationCanceledException)
{
    // Animation was cancelled, view position is partial
    // Safe to reset or update
}
```

**Composing Animations:**

```csharp
// Sequential
async Task AnimateSequential()
{
    await view1.FadeToAsync(0, 300);
    await view2.FadeToAsync(1, 300);
    await view3.FadeToAsync(0.5, 300);
}

// Parallel
async Task AnimateParallel()
{
    await Task.WhenAll(
        view1.ScaleToAsync(1.5, 300),
        view2.TranslateToAsync(100, 0, 300),
        view3.RotateToAsync(360, 300)
    );
}

// Complex timing
async Task AnimateComplex()
{
    // Fade in views
    await Task.WhenAll(
        view1.FadeToAsync(1, 200),
        view2.FadeToAsync(1, 200)
    );
    
    // Then scale
    await view1.ScaleToAsync(1.1, 300, easing: Easing.SpringOut);
}
```

**Migration Pragmatism:**

Synchronous methods still compile (with compiler warnings). Migrate incrementally:

```csharp
#pragma warning disable CS0618  // Obsolete member
view.FadeTo(0, 500);  // Still works, but discouraged
#pragma warning restore CS0618
```

---

### Control Improvements: Quality-of-Life Updates

**HybridWebView / BlazorWebView: Request Interception**

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
    if (e.Uri.Host.Contains("analytics-tracker.com") || 
        e.Uri.Host.Contains("facebook.com/tr"))
    {
        e.Handled = true;
        e.SetResponse(204, "No Content", "text/plain", null);
    }
};
```

**Critical Implementation Rules:**

- Handler MUST be synchronous (called on WebView thread, not UI thread)
- If you set `Handled = true`, MUST provide response via `SetResponse()` (even null content)
- Do NOT perform long-running operations (file I/O, network requests); WebView will freeze
- For async content, pass `Task<Stream>` to SetResponse(); WebView waits for completion

**New Events: WebViewInitializing / WebViewInitialized**

Platform-specific initialization hooks (matching BlazorWebView pattern):

```csharp
webView.WebViewInitializing += (s, e) =>
{
    // Before native WebView created
    // Configure iOS-specific settings
    if (DeviceInfo.Platform == DevicePlatform.iOS)
    {
        // Configure NSUrlSessionConfiguration, etc.
    }
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

**JavaScript Exceptions: Automatic Rethrow**

By default, exceptions thrown in JavaScript are re-thrown as .NET exceptions:

```javascript
// JavaScript
throw new Error("User validation failed");
```

```csharp
// .NET
try
{
    await webView.InvokeJavaScriptAsync("doSomething", "arg1");
}
catch (Exception ex)
{
    // ex.Message = "User validation failed"
}
```

**MediaPicker Enhancements**

```csharp
// Multiple file selection
var photos = await MediaPicker.PickMultipleAsync(new MediaPickerOptions
{
    Title = "Select up to 10 photos"
});

foreach (var photo in photos)
{
    var stream = await photo.OpenReadAsync();
    await UploadPhotoAsync(stream);
}

// Automatic image compression
var result = await MediaPicker.PickMultipleAsync(new MediaPickerOptions
{
    MaximumWidth = 1024,    // Downscale to max 1024px
    MaximumHeight = 768,    // Downscale to max 768px
});

// EXIF auto-rotation + preservation
var photo = await MediaPicker.PickPhotoAsync();
// Photo is automatically rotated based on device orientation metadata
// EXIF data preserved for camera/timestamp info
```

**Platform-Specific: WebView Fullscreen Video on Android**

```html
<!-- HTML embedded video -->
<iframe 
    src="https://example.com/video.html"
    allowfullscreen>
</iframe>
```

Previously, tapping fullscreen on Android would fail. MAUI 10 properly handles the fullscreen lifecycle via WebChromeClient overrides.

**SearchBar Additions**

```xaml
<SearchBar Placeholder="Search..."
           SearchIconColor="Blue"
           ReturnType="Search" />
```

New properties:
- `SearchIconColor`: Color of the search magnifying glass icon
- `ReturnType`: Appearance of return/search button (Search, Go, Done, Send, Next, Previous)

**Switch: OffColor**

```xaml
<Switch OffColor="Red"
        OnColor="Green" />
```

Control color when switch is OFF (previously only OnColor available).

**RefreshView: IsRefreshEnabled**

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

**Picker: Programmatic Open/Close**

```csharp
// Open picker programmatically
picker.Open();

// Close picker programmatically
picker.Close();
```

**Text-to-Speech: Rate Control**

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

**WebAuthenticator: CancellationToken Support**

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
    // User took too long, cancelled, or deep linking interrupted
}
```

**Vibration / HapticFeedback: IsSupported Check**

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

**Geolocation: IsEnabled Property**

```csharp
if (!Geolocation.IsEnabled)
{
    await DisplayAlert("Location", "Please enable location services", "OK");
    return;
}

var location = await Geolocation.GetLocationAsync();
```

**TabbedPageManager / StackNavigationManager: Now Public**

Advanced scenario support:

```csharp
#if ANDROID
// Customize tab navigation behavior on Android
var manager = TabbedPage.GetManager(this) as TabbedPageManager;
manager.UpdateCurrentPage(newIndex);
#endif
```

---

## Part IV: Platform-Specific Features

### iOS and Mac Catalyst: Popovers and Secondary Toolbars

**Modal Popovers (New)**

iOS/Mac Catalyst specific: display modal pages as popovers anchored to a view:

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
var originButton = findButton;
var popover = new PopoverPage(originButton, Rectangle.Empty);
await Navigation.PushModalAsync(popover);
```

Use cases:
- Context menus
- Filter options
- Quick settings from toolbar button
- Non-intrusive editing workflows

**Secondary Toolbar Items (New)**

Modern iOS 13+ pattern: secondary items hidden in pull-down menu:

```xaml
<ContentPage.ToolbarItems>
    <!-- Primary items appear directly -->
    <ToolbarItem Text="Save" Order="Primary" Priority="0" 
                  Clicked="OnSaveClicked" />
    <ToolbarItem Text="Share" Order="Primary" Priority="1"
                  Clicked="OnShareClicked" />
    
    <!-- Secondary items appear in overflow menu (iOS) or 3-dot menu (Android) -->
    <ToolbarItem Text="Delete" Order="Secondary" Priority="0"
                  Clicked="OnDeleteClicked" />
    <ToolbarItem Text="Export" Order="Secondary" Priority="1"
                  Clicked="OnExportClicked" />
    <ToolbarItem Text="Settings" Order="Secondary" Priority="2"
                  Clicked="OnSettingsClicked" />
</ContentPage.ToolbarItems>
```

Behavior by platform:
- **iOS/Mac Catalyst:** Secondary items grouped in system ellipsis menu (UIImage.GetSystemImage("ellipsis.circle")), pull-down reveals vertical list ordered by Priority
- **Android:** Three-dot overflow menu, vertical list
- **Windows:** Standard menu
- Menu auto-rebuilds and closes when item properties change

**SafeArea Enhancements**

Granular control over safe area behavior with `SafeAreaEdges` property on Layout, ContentView, ContentPage, Border, ScrollView:

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
<!-- Flow under keyboard but respect notch/bars -->
<ContentPage SafeAreaEdges="Container">
    <ScrollView SafeAreaEdges="None">
        <!-- Edge-to-edge content inside scrollable area -->
    </ScrollView>
</ContentPage>

<Grid SafeAreaEdges="SoftInput">
    <!-- Grid respects keyboard but ignores notch -->
</Grid>
```

Platform-specific fixes in RC1:
- iOS ScrollView no longer adds extra bottom space
- Safe area calculation improved for edge cases
- Mac Catalyst keyboard handling aligned with iOS

---

### Android 16 (API 36) and Performance Optimizations

**API 36 Default**

.NET MAUI 10 targets Android 16 (API 36) by default when using `net10.0-android`:

```xml
<PropertyGroup>
  <TargetFramework>net10.0-android</TargetFramework>
  <!-- Automatically uses API 36 -->
</PropertyGroup>
```

**Recommended Minimum: API 24 (Nougat)**

Templates now default to `SupportedOSPlatformVersion=24` instead of 21:

```xml
<PropertyGroup>
  <SupportedOSPlatformVersion>24</SupportedOSPlatformVersion>
</PropertyGroup>
```

Why: Java 8+ (used by modern Android) includes default interface methods. "Desugaring" (converting to pre-Java8 bytecode for older APIs) is unreliable. API 24+ has native support.

**Core CLR (Experimental)**

Enable CoreCLR runtime instead of Mono for potential performance improvements:

```xml
<PropertyGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">
    <UseMonoRuntime>false</UseMonoRuntime>
</PropertyGroup>
```

Real-world startup improvement reported by community users:
- CoreCLR vs Mono: Significantly faster startup (one developer reported over 72% improvement)
- CoreCLR + NativeAOT vs Mono: Even more substantial improvements (over 125% reported)
- Trade-off: App size currently larger; debugging incomplete

Status: Experimental. Not recommended for production. Report issues with `UseMonoRuntime=false` specified.

**Marshal Methods Enabled by Default**

Performance optimization for Java→C# calls enabled by default (was disabled in .NET 9):

```csharp
// Safe to keep enabled for most projects
// If you see startup hangs:
<PropertyGroup>
    <AndroidEnableMarshalMethods>false</AndroidEnableMarshalMethods>
</PropertyGroup>
```

**Design-Time Build Performance (Major Win)**

Historically, `aapt2` (Android resource processor) ran during IDE design-time builds, causing multi-second delays for IntelliSense.

MAUI 10 parses .aar files directly:
- Before: Multi-second delays for design-time build
- After: Sub-second design-time build
- Impact: IntelliSense feels instant

**APK/AAB Creation Speed**

Command-line builds now use `System.IO.Compression` instead of `libzipsharp`:
- Faster APK/AAB generation
- Visual Studio still uses libzipsharp (no alternative in .NET Framework)

**dotnet run Support**

Deploy directly to devices without Visual Studio:

```bash
# Run on attached physical device
dotnet run -p:AdbTarget=-d

# Run on emulator
dotnet run -p:AdbTarget=-e

# Run on specific device
dotnet run -p:AdbTarget="-s emulator-5554"
```

**JDK 21 Support**

Build with modern Java tooling:

```bash
java -version
# openjdk 21.x.x or higher
```

**AndroidMavenLibrary ArtifactFilename**

Bind Java libraries from Maven with custom filenames:

```xml
<ItemGroup>
    <AndroidMavenLibrary Include="com.facebook.react:react-android"
                          Version="0.76.0"
                          ArtifactFilename="react-android-0.76.0-release.aar" />
</ItemGroup>
```

---

### iOS, Mac Catalyst, tvOS: Trimming and NativeAOT

**Trimmer Now Enabled by Default for Simulators**

Configuration | .NET 9 | .NET 10
---|---|---
iOS Simulator (arm64) | Disabled | **Enabled**
tvOS Simulator (arm64) | Disabled | **Enabled**
Mac Catalyst (arm64) | Disabled | **Enabled**
iOS Device (arm64) | Enabled | Enabled (unchanged)

This enables testing trimming/AOT issues in simulator before device deployment.

**Trimmer Warnings Enabled by Default**

Previously suppressed because BCL generated warnings. Fixed in .NET 9, so now enabled in MAUI 10:

```
$ dotnet build -c Release
...
[IL2104] Member 'MyClass.MyMethod' is referenced
         but implements a method that isn't annotated.
```

Suppress if necessary (not recommended):

```xml
<PropertyGroup>
    <SuppressTrimAnalysisWarnings>true</SuppressTrimAnalysisWarnings>
</PropertyGroup>
```

**Build Binding Projects on Windows (Major Improvement)**

Previously required remote Mac for resource compilation (storyboards, xibs). MAUI 10 processes these on Windows:

- Before: Connect to Mac, remote build, compile resources on Mac (slow)
- After: Compile entirely on Windows (multiple times faster)

Enables CI/CD on Windows runners without Mac VMs.

**Bundle Original Resources: Opt-Out (Default Changed)**

MAUI 9 introduced opt-in bundling of original XAML/storyboard resources in libraries. MAUI 10 reverses:

Default (opt-out):
```xml
<!-- Automatically enabled in .NET 10 -->
<PropertyGroup>
    <BundleOriginalResources>true</BundleOriginalResources>
</PropertyGroup>
```

Opt-out if needed:
```xml
<PropertyGroup>
    <BundleOriginalResources>false</BundleOriginalResources>
</PropertyGroup>
```

**NSUrlSessionHandler TLS Change**

BREAKING: `NSUrlSessionHandler` no longer reads `ServicePointManager.SecurityProtocol`:

Before (.NET 9):
```csharp
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
var handler = new NSUrlSessionHandler();
// TLS 1.3 automatically used
```

After (.NET 10):
```csharp
var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
config.TlsMinimumSupportedProtocolVersion = TlsProtocolVersion.Tls13;
var handler = new NSUrlSessionHandler(config);
```

Rationale: `ServicePointManager` is deprecated. Set TLS directly on `NSUrlSessionConfiguration`.

**Xcode 26 Beta 4 Support**

.NET 10 Preview 7 included Xcode 26 Beta 4 bindings. From RC1, iOS 18.2 bindings are stable:

```xml
<PropertyGroup>
  <TargetFramework>net10.0-ios18.2</TargetFramework>
  <!-- net10.0-maccatalyst18.2 also available -->
</PropertyGroup>
```

**CollectionView / CarouselView: Optimized Handlers Default**

Handlers available in .NET 9 as optional are now default in MAUI 10:

Platform | .NET 9 | .NET 10 | Benefit
---|---|---|---
iOS | Optional | **Default** | Better performance, memory
Mac Catalyst | Optional | **Default** | Better performance, memory
Android | Standard | Standard | Unchanged
Windows | Standard | Standard | Unchanged

Opt-out (revert to .NET 9 handler) if needed:

```csharp
#if IOS || MACCATALYST
builder.ConfigureMauiHandlers(handlers =>
{
    handlers.AddHandler<CollectionView, CollectionViewHandler>();
    handlers.AddHandler<CarouselView, CarouselViewHandler>();
});
#endif
```

**Native AOT Performance Benefits**

Native AOT deployment for a `dotnet new maui` app on iOS and Mac Catalyst produces:

- **App package size:** Typically up to 2.5x smaller apps compared to the default deployment model
- **Startup time:** Typically up to 2x faster startup times on iOS devices and 1.2x faster on Mac Catalyst compared to Mono deployment
- **Build time:** Typically up to 2.8x faster build times on iOS devices compared to the default deployment model

Important: These are typical results for the default template. Actual performance benefits are hardware-dependent and will vary by application complexity.

---

## Part V: Deprecations and Migration

### Control Deprecations

| Deprecated | Replacement | Timeline |
|---|---|---|
| `ListView` | `CollectionView` | Fully deprecated |
| `TableView` | `CollectionView` | Fully deprecated |
| `EntryCell`, `ImageCell`, `SwitchCell`, `TextCell`, `ViewCell` | `CollectionView` + templates | Fully deprecated |
| `Cell` (base class) | Still used for source gen, but consider deprecated | Deprecation pending |
| `Accelerator` | `KeyboardAccelerator` | Removed |
| `ClickGestureRecognizer` | `TapGestureRecognizer` | Removed |
| `MessagingCenter` | `WeakReferenceMessenger` (CommunityToolkit.Mvvm) | Made internal |

**ListView → CollectionView Migration:**

```csharp
// Old (deprecated)
<ListView ItemsSource="{Binding Items}"
          HasUnevenRows="true">
    <ListView.ItemTemplate>
        <DataTemplate>
            <ViewCell>
                <Label Text="{Binding Name}" />
            </ViewCell>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>

// New (recommended)
<CollectionView ItemsSource="{Binding Items}"
                SelectionMode="Single"
                SelectionChangedCommand="{Binding SelectItemCommand}"
                SelectionChangedCommandParameter="{Binding ., Source={RelativeSource Self}}">
    <CollectionView.ItemsLayout>
        <LinearItemsLayout Orientation="Vertical" ItemSpacing="5" />
    </CollectionView.ItemsLayout>
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Label Text="{Binding Name}" />
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

### Animation Method Deprecations

All 11 synchronous animation methods deprecated. Compile with warnings visible to identify usages.

### Pop-Up Deprecations

```csharp
// Old
await DisplayAlert("Title", "Message", "OK");
await DisplayActionSheet("Title", "Cancel", "Destroy", "Option1", "Option2");

// New
await DisplayAlertAsync("Title", "Message", "OK");
var result = await DisplayActionSheetAsync("Title", "Cancel", "Destroy", "Option1", "Option2");
```

### Page.IsBusy Obsoleted

Replace with explicit loading indicators:

```xaml
<!-- Old way -->
<ContentPage IsBusy="true">
    <!-- Content hidden during busy -->
</ContentPage>

<!-- New way -->
<Grid RowDefinitions="*,50" ColumnDefinitions="*">
    <ScrollView Grid.Row="0">
        <!-- Actual content -->
    </ScrollView>
    
    <ActivityIndicator Grid.Row="1"
                       IsRunning="{Binding IsLoading}"
                       IsVisible="{Binding IsLoading}" />
</Grid>
```

### FontImageExtension Removed

```xaml
<!-- Old (deprecated) -->
<Button ImageSource="{FontImageExtension Glyph=MyIcon, FontFamily=FontAwesome}" />

<!-- New (recommended) -->
<Button>
    <Button.ImageSource>
        <FontImageSource Glyph="MyIcon"
                         FontFamily="FontAwesome"
                         Color="Blue"
                         Size="24" />
    </Button.ImageSource>
</Button>
```

---

---
name: .NET MAUI - Aspire Integration
description: "This sample demonstrates how to integrate .NET MAUI applications with Aspire for simplified service orchestration, discovery, and development. It showcases connecting a MAUI app to a web API using Aspire's App Host and service defaults."
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
- dotnet-aspire
urlFragment: maui-aspire-integration
---

# .NET MAUI with Aspire Integration

This sample demonstrates the integration between .NET MAUI and Aspire, showcasing how Aspire simplifies connecting mobile and desktop applications to local web services during development.

## What is Aspire?

Aspire provides a consistent, opinionated set of tools and patterns for building and running distributed applications. It's designed to improve the experience of building cloud-native applications by providing:

- **Orchestration**: Simplified management of multiple services and dependencies
- **Components**: Pre-built integrations for common services and platforms
- **Tooling**: Developer dashboard for monitoring and managing services
- **Service discovery**: Automatic configuration for service-to-service communication

## Why Use Aspire with .NET MAUI?

Integrating Aspire with your .NET MAUI applications provides several key benefits:

- **Simplified configuration**: Eliminate complex platform-specific networking configuration. No need to manually handle `10.0.2.2` for Android or deal with certificate validation issues.
- **Automatic service discovery**: Your MAUI app automatically discovers and connects to local services without hardcoded URLs.
- **Development tunnels integration**: Built-in support for Dev Tunnels on iOS and Android, making it easy to connect mobile emulators and simulators to local services.
- **Consistent experience**: Use the same patterns and tools across your entire application stack.
- **Observable services**: Monitor your services through the Aspire dashboard during development.

## Platform Support

Aspire integration with .NET MAUI supports all platforms:

- **Windows**
- **Mac Catalyst**
- **iOS**
- **Android**

> [!IMPORTANT]
> This feature is currently in preview. Some features are still being implemented, and integration with Visual Studio 2026 is not completely available yet.

## Prerequisites

To use this sample or integrate Aspire with your own .NET MAUI project, you need:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [.NET MAUI workload](https://learn.microsoft.com/dotnet/maui/get-started/installation) installed
- [Aspire 13](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling) or later

## What's in This Sample?

This sample contains a complete working example with:

- **SampleMauiApp**: A .NET MAUI application that consumes a weather API
- **SampleWebApi**: An ASP.NET Core Web API providing weather data
- **MauiApp.ServiceDefaults**: Service defaults configuration for the MAUI app
- **MauiAspire.ServiceDefaults**: Service defaults configuration for the web API
- **MauiAspire.AppHost**: The Aspire App Host that orchestrates all services
- **SampleBlazorHybridApp**: Additional Blazor Hybrid MAUI sample showing Aspire integration
- **SampleHybridWebViewApp**: Additional HybridWebView MAUI sample showing Aspire integration

## Running the Sample

You can run this sample using Visual Studio, Visual Studio Code, or the command line.

### Visual Studio

1. Open the solution in Visual Studio 2022 or later
2. Set `MauiAspire.AppHost` as the startup project
3. Press F5 to run the application
4. The Aspire dashboard will open in your browser, showing all registered services
5. From the dashboard, manually start the MAUI app targets (Windows, Mac Catalyst, iOS, or Android)
6. The MAUI app will connect to the weather API through service discovery

### Visual Studio Code

1. Open the `MauiAspire.AppHost` folder in VS Code
2. Run the project using the debugger or the Run/Debug panel
3. The Aspire dashboard will open in your browser
4. From the dashboard, manually start the MAUI app targets

### Command Line

Navigate to the `MauiAspire.AppHost` directory and run one of the following commands:

**Using dotnet run:**
```bash
dotnet run
```

**Using aspire run:**
```bash
aspire run
```

Both commands will start the Aspire dashboard. From there, you can manually start the MAUI app targets for your desired platforms.

## Applying Aspire to Your Existing .NET MAUI Project

Follow these steps to add Aspire integration to your own .NET MAUI application:

### 1. Update Your .NET MAUI Project

Ensure your .NET MAUI app is using:
- `Microsoft.Maui.Controls` .NET 10 RC2 or later
- Target framework: `net10.0` or later

### 2. Create the MAUI Service Defaults Project

The MAUI Service Defaults project contains shared configuration for service discovery, telemetry, and resilience patterns.

```bash
dotnet new maui-aspire-servicedefaults -n YourApp.MauiServiceDefaults
```

Add a reference from your MAUI app to the Service Defaults project:

```bash
dotnet add YourMauiApp.csproj reference YourApp.MauiServiceDefaults/YourApp.MauiServiceDefaults.csproj
```

> [!TIP]
> To enable detailed metrics and tracing from the .NET MAUI SDK, open the `Extensions.cs` file in your MAUI Service Defaults project and uncomment the lines that add `Microsoft.Maui` as a meter and tracing source. Note that this may generate a significant amount of telemetry data.

### 3. Create the Aspire App Host Project

The App Host project orchestrates all your application services.

```bash
dotnet new aspire-apphost -n YourApp.AppHost
```

Add references to your MAUI app and web service projects:

```bash
dotnet add YourApp.AppHost.csproj reference YourWebService/YourWebService.csproj
```

> [!NOTE]
> Unlike traditional project references, MAUI projects are added using the `AddMauiProject` method in code (see next step), not as a project reference in the `.csproj` file.

### 4. Configure the App Host

In your App Host project's `AppHost.cs` or `Program.cs`, register your services and MAUI app:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Register your web service
var webapi = builder.AddProject<Projects.SampleWebApi>("webapi");

// Register your MAUI app using the path to the .csproj file
var mauiapp = builder.AddMauiProject("mauiapp", "../YourMauiApp/YourMauiApp.csproj");

// Add Windows device (uses localhost directly)
mauiapp.AddWindowsDevice()
    .WithReference(webapi);

// Add Mac Catalyst device (uses localhost directly)
mauiapp.AddMacCatalystDevice()
    .WithReference(webapi);

// Coming soon: iOS and Android support
// mauiapp.AddiOSSimulator()
//     .WithOtlpDevTunnel()
//     .WithReference(webapi, publicDevTunnel);

builder.Build().Run();
```

### 5. Configure Your MAUI App

In your MAUI app's `MauiProgram.cs`, add service defaults and configure HTTP clients:

```csharp
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

        // Add service defaults - this enables service discovery and telemetry
        builder.AddServiceDefaults();

        // Configure HTTP client with service discovery
        builder.Services.AddHttpClient<WeatherApiClient>(client =>
        {
            // Service name must match the name used in App Host
            // The https+http:// scheme prefers HTTPS but falls back to HTTP
            client.BaseAddress = new Uri("https+http://webapi");
        });

        return builder.Build();
    }
}
```

### 6. Create a Service Client

Create a typed client to consume your web service:

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
```

### 7. Use the Client in Your App

Inject and use the client in your pages or view models:

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
                // Display the weather data
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

### 8. Run Your Application

Run the App Host project using your preferred method:
- **Visual Studio**: Set the App Host project as the startup project and press F5
- **VS Code**: Open the AppHost folder and run/debug the project
- **Command line**: Navigate to the App Host directory and run `dotnet run` or `aspire run`

When running:
1. The Aspire dashboard will open at `https://localhost:[port]`
2. From the dashboard, manually start the MAUI app targets for your desired platforms
3. Your MAUI app will connect to services through service discovery
4. Monitor logs, traces, and metrics in the Aspire dashboard

## Platform-Specific Considerations

### Windows and Mac Catalyst

On Windows and Mac Catalyst, the Aspire integration works seamlessly using localhost. Service discovery automatically provides the correct URLs for connecting to local services.

### iOS and Android

iOS and Android are fully supported. Dev Tunnels are automatically configured to enable connectivity between mobile emulators/simulators/devices and services running on your development machine.

## Features Demonstrated

This sample showcases:

- ✅ Complete project structure for Aspire + MAUI
- ✅ Service registration and discovery
- ✅ HTTP client configuration with service discovery
- ✅ Telemetry and monitoring through the Aspire dashboard
- ✅ Windows and Mac Catalyst platform support
- ✅ Error handling and resilience patterns

## Troubleshooting

### Service Discovery Not Working

If your MAUI app can't connect to services:

1. Verify `AddServiceDefaults()` is called in `MauiProgram.cs`
2. Ensure the service name in your HTTP client matches the App Host registration
3. Check that you're using the `https+http://` scheme
4. Verify all projects build successfully

### App Host Won't Start

If the App Host fails to start:

1. Ensure all project paths in `AppHost.cs` are correct
2. Verify .NET 10 SDK and Aspire workload are installed
3. Check the App Host console output for error messages
4. Ensure your web service projects target a compatible framework

### MAUI App Can't Find Service Defaults

If you see errors about missing service defaults:

1. Verify the Service Defaults project reference in your MAUI `.csproj`
2. Ensure the Service Defaults project builds successfully
3. Clean and rebuild the solution

## Additional Resources

For more information about this sample and the technologies it uses, see:

- [Aspire overview](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview)
- [Aspire service discovery](https://learn.microsoft.com/dotnet/aspire/service-discovery/overview)
- [Aspire app host](https://learn.microsoft.com/dotnet/aspire/fundamentals/app-host-overview)
- [Connect to local web services from iOS simulators and Android emulators](https://learn.microsoft.com/dotnet/maui/data-cloud/local-web-services)
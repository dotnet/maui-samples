# Extending Telepathy with Foundry Local: Cloud ↔ On-Device AI Toggle

> A complete guide to adding Microsoft Foundry Local (on-device inference) alongside Microsoft Foundry (cloud) in the Telepathy .NET MAUI app, using `IChatClient` and dependency injection to swap providers at runtime.

---

## 1. Architecture Overview

Telepathy already uses the **`IChatClientService`** abstraction to decouple AI operations from specific providers. The existing `ChatClientService` supports two providers today — **OpenAI** and **Microsoft Foundry** (cloud). This guide adds a third: **Foundry Local** (on-device).

### The Pattern

```
┌──────────────────┐
│   PageModel /    │  Injects IChatClientService — no knowledge of provider
│   ViewModel      │
└────────┬─────────┘
         │
┌────────▼─────────┐
│ IChatClientService│  App-specific wrapper with UpdateClient(), GetClient()
└────────┬─────────┘
         │
┌────────▼─────────┐
│   IChatClient     │  Microsoft.Extensions.AI unified interface
└────────┬─────────┘
         │
    ┌────┴────────────────┬──────────────────────┐
    │                     │                      │
┌───▼───────┐   ┌────────▼──────┐   ┌───────────▼────────┐
│  OpenAI   │   │ Azure AI      │   │  Foundry Local     │
│ (cloud)   │   │ Foundry       │   │  (on-device)       │
│           │   │ (cloud)       │   │  phi-4-mini, etc.  │
└───────────┘   └───────────────┘   └────────────────────┘
```

**Key insight:** Foundry Local exposes an **OpenAI-compatible REST API** on `localhost`. Once you have the local endpoint URL and API key from `FoundryLocalManager`, you create a standard `OpenAIClient` pointed at that endpoint. The resulting `IChatClient` is identical in shape to the cloud version — your ViewModels never know the difference.

### What Changes

| Layer | Cloud (AI Foundry) | Local (Foundry Local) |
|-------|-------------------|-----------------------|
| **SDK** | `AzureOpenAIClient` | `FoundryLocalManager` → `OpenAIClient` |
| **Endpoint** | `https://your-resource.openai.azure.com` | `http://127.0.0.1:{port}/v1` |
| **Model** | `gpt-4o`, `gpt-4o-mini` | `phi-4-mini`, `phi-4` |
| **Auth** | Azure API key | Local API key (from manager) |
| **IChatClient** | Same interface | Same interface |

---

## 2. Project File Changes

### Telepathic.csproj — NuGet Packages

Add the Foundry Local package alongside the existing packages. Use **conditional references** because Foundry Local currently supports Windows and macOS only (not iOS/Android).

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <!-- ... existing PropertyGroup ... -->

  <ItemGroup>
    <!-- ============================================================ -->
    <!-- EXISTING PACKAGES (already in the project)                   -->
    <!-- ============================================================ -->
    <PackageReference Include="Azure.AI.OpenAI" Version="2.1.0" />
    <PackageReference Include="Microsoft.Extensions.AI" Version="9.9.1" />
    <PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.4.4-preview.1.25259.16" />
    <!-- ... other packages ... -->
  </ItemGroup>

  <!-- ============================================================== -->
  <!-- NEW: Foundry Local — only for platforms that support it         -->
  <!-- ============================================================== -->

  <!-- Windows: use the WinML-accelerated package for GPU/NPU inference -->
  <ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
    <PackageReference Include="Microsoft.AI.Foundry.Local.WinML" Version="0.9.*" />
  </ItemGroup>

  <!-- macOS (Mac Catalyst): uses Foundry Local CLI via REST API.      -->
  <!-- No NuGet package needed — the CLI runs as a separate process.   -->
  <!-- Install with: brew install microsoft/foundrylocal/foundrylocal  -->

  <!-- iOS / Android: Foundry Local is NOT supported.                 -->
  <!-- The code uses #if preprocessor directives to exclude local AI  -->
  <!-- functionality on these platforms. No package reference needed.  -->

</Project>
```

> **Platform notes:**
> - **Windows**: `Microsoft.AI.Foundry.Local.WinML` enables in-process inference with DirectML GPU/NPU acceleration.
> - **macOS (Mac Catalyst)**: No NuGet package. The app discovers the Foundry Local CLI's REST endpoint at runtime via `foundry service status`. Install the CLI with `brew install microsoft/foundrylocal/foundrylocal`.
> - **iOS / Android**: Foundry Local is not supported on these platforms today. The app falls back to cloud AI. (On iOS, you can use `Microsoft.ML.OnnxRuntimeGenAI` directly for on-device inference.)

---

## 3. Service Abstraction — Updated `IChatClientService`

The existing `IChatClientService` interface already has `UpdateClient(apiKey, provider, endpoint, model)`. We extend the **implementation** to handle a `"local"` provider, adding model lifecycle management.

### 3a. New Interface Members

Add these to `IChatClientService`:

```csharp
// file: Services/IChatClientService.cs (additions to existing interface)

public interface IChatClientService
{
    // ... existing members ...

    /// <summary>
    /// Gets the current provider type ("openai", "foundry", or "local")
    /// </summary>
    string CurrentProvider { get; }

    /// <summary>
    /// Whether Foundry Local is available on this platform
    /// </summary>
    bool IsLocalAvailable { get; }

    /// <summary>
    /// Initializes the Foundry Local provider. Downloads the model on first run.
    /// </summary>
    /// <param name="modelAlias">Model alias, e.g. "phi-4-mini"</param>
    /// <param name="progress">Optional progress callback for model download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InitializeLocalAsync(
        string modelAlias = "phi-4-mini",
        IProgress<FoundryLocalDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Switches between cloud and local providers at runtime
    /// </summary>
    /// <param name="useLocal">true for Foundry Local, false for cloud</param>
    Task ToggleProviderAsync(bool useLocal);
}
```

### 3b. Download Progress Model

```csharp
// file: Services/FoundryLocalDownloadProgress.cs

namespace Telepathic.Services;

/// <summary>
/// Progress information for Foundry Local model downloads.
/// Models are typically 1-3 GB and download on first use.
/// </summary>
public record FoundryLocalDownloadProgress(
    string ModelAlias,
    double PercentComplete,
    long BytesDownloaded,
    long TotalBytes,
    string Status // "downloading", "extracting", "ready"
);
```

### 3c. Updated `ChatClientService` Implementation

Here is the complete updated service. The key addition is the `"local"` case in `UpdateClient` and the `InitializeLocalAsync` method.

```csharp
// file: Services/ChatClientService.cs

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Telepathic.Tools;
using OpenAI;
using Azure.AI.OpenAI;

// Foundry Local is only available on Windows and macOS
#if WINDOWS || MACCATALYST
using Microsoft.AI.Foundry.Local;
#endif

namespace Telepathic.Services;

public class ChatClientService : IChatClientService
{
    private IChatClient? _chatClient;
    private AzureOpenAIClient? _azureOpenAIClient;
    private readonly ILogger _logger;
    private readonly LocationTools _locationTools;
    private IList<object>? _cachedTools;

    // ── NEW: Track current provider and local model state ──
    public string CurrentProvider { get; private set; } = "none";

#if WINDOWS || MACCATALYST
    // Foundry Local is available on desktop platforms
    public bool IsLocalAvailable => true;
    private FoundryLocalManager? _localManager;
    private string? _localModelId;
#else
    // Not available on iOS or Android
    public bool IsLocalAvailable => false;
#endif

    // Store the last cloud config so we can toggle back
    private string? _lastCloudApiKey;
    private string? _lastCloudProvider;
    private string? _lastCloudEndpoint;
    private string? _lastCloudModel;

    public ChatClientService(ILogger<ChatClientService> logger, LocationTools locationTools)
    {
        _logger = logger;
        _locationTools = locationTools;

        // Try to initialize from preferences (existing logic)
        var foundryEndpoint = Preferences.Default.Get("foundry_endpoint", string.Empty);
        var foundryApiKey = Preferences.Default.Get("foundry_api_key", string.Empty);
        var openAiApiKey = Preferences.Default.Get("openai_api_key", string.Empty);

        if (!string.IsNullOrEmpty(foundryEndpoint) && !string.IsNullOrEmpty(foundryApiKey))
        {
            UpdateClient(foundryApiKey, "foundry", foundryEndpoint);
        }
        else if (!string.IsNullOrEmpty(openAiApiKey))
        {
            UpdateClient(openAiApiKey);
        }
    }

    public IChatClient GetClient()
    {
        return _chatClient
            ?? throw new InvalidOperationException(
                "Chat client has not been initialized. Please provide an API key first.");
    }

    public AzureOpenAIClient GetAzureOpenAIClient()
    {
        return _azureOpenAIClient
            ?? throw new InvalidOperationException(
                "Azure OpenAI client has not been initialized.");
    }

    public bool IsInitialized => _chatClient != null;

    // ── Existing methods (GetMcpToolsAsync, GetResponseWithToolsAsync) unchanged ──
    // ... (see existing code) ...

    // ================================================================
    // UpdateClient — now with "local" provider support
    // ================================================================

    public void UpdateClient(string apiKey, string model = "gpt-4o-mini")
    {
        // Existing OpenAI logic (unchanged)
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Attempted to update chat client with empty API key");
            _chatClient = null;
            return;
        }

        try
        {
            var openAIClient = new OpenAIClient(apiKey);
            _chatClient = openAIClient.GetChatClient(model: model).AsIChatClient();
            _chatClient = WrapWithToolsAndLogging(_chatClient);
            CurrentProvider = "openai";

            // Save for toggle-back
            _lastCloudApiKey = apiKey;
            _lastCloudProvider = "openai";
            _lastCloudModel = model;

            _cachedTools = null;
            _logger.LogInformation("Chat client initialized: OpenAI, model={Model}", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update chat client");
            _chatClient = null;
            throw;
        }
    }

    public void UpdateClient(
        string apiKey,
        string provider,
        string? endpoint = null,
        string model = "gpt-4o")
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Attempted to update chat client with empty API key");
            _chatClient = null;
            return;
        }

        try
        {
            switch (provider.ToLowerInvariant())
            {
                case "foundry":
                    if (string.IsNullOrEmpty(endpoint))
                        throw new ArgumentException(
                            "Foundry provider requires an endpoint URL", nameof(endpoint));

                    _azureOpenAIClient = new AzureOpenAIClient(
                        new Uri(endpoint),
                        new System.ClientModel.ApiKeyCredential(apiKey));
                    _chatClient = _azureOpenAIClient
                        .GetChatClient(model)
                        .AsIChatClient();

                    CurrentProvider = "foundry";

                    // Save for toggle-back
                    _lastCloudApiKey = apiKey;
                    _lastCloudProvider = "foundry";
                    _lastCloudEndpoint = endpoint;
                    _lastCloudModel = model;
                    break;

                // ────────────────────────────────────────────────────
                // NEW: Foundry Local provider
                // ────────────────────────────────────────────────────
                case "local":
#if WINDOWS || MACCATALYST
                    // apiKey and endpoint come from FoundryLocalManager
                    // (set up by InitializeLocalAsync)
                    if (string.IsNullOrEmpty(endpoint))
                        throw new ArgumentException(
                            "Local provider requires the FoundryLocalManager endpoint");

                    var localOpenAI = new OpenAIClient(
                        new System.ClientModel.ApiKeyCredential(apiKey),
                        new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
                    _chatClient = localOpenAI
                        .GetChatClient(model)
                        .AsIChatClient();

                    CurrentProvider = "local";
                    _logger.LogInformation(
                        "Chat client initialized: Foundry Local, model={Model}, endpoint={Endpoint}",
                        model, endpoint);
                    break;
#else
                    throw new PlatformNotSupportedException(
                        "Foundry Local is not supported on this platform. " +
                        "Use cloud AI (Microsoft Foundry or OpenAI) instead.");
#endif

                case "openai":
                default:
                    var openAIClient = new OpenAIClient(
                        new System.ClientModel.ApiKeyCredential(apiKey));
                    _chatClient = openAIClient
                        .GetChatClient(model: model)
                        .AsIChatClient();

                    CurrentProvider = "openai";
                    _lastCloudApiKey = apiKey;
                    _lastCloudProvider = "openai";
                    _lastCloudModel = model;
                    break;
            }

            _chatClient = WrapWithToolsAndLogging(_chatClient);
            _cachedTools = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update chat client for provider: {Provider}", provider);
            _chatClient = null;
            throw;
        }
    }

    // ================================================================
    // NEW: Initialize Foundry Local — downloads model on first run
    // ================================================================

    public async Task InitializeLocalAsync(
        string modelAlias = "phi-4-mini",
        IProgress<FoundryLocalDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
#if WINDOWS || MACCATALYST
        try
        {
            _logger.LogInformation("Starting Foundry Local model: {Model}", modelAlias);

            progress?.Report(new FoundryLocalDownloadProgress(
                modelAlias, 0, 0, 0, "downloading"));

            // StartModelAsync handles:
            //   1. Starting the local inference service if not running
            //   2. Downloading the model if not cached (~1-3 GB first time)
            //   3. Loading the model into memory
            _localManager = await FoundryLocalManager.StartModelAsync(
                aliasOrModelId: modelAlias);

            // Get model details for the correct model ID
            var modelInfo = await _localManager.GetModelInfoAsync(modelAlias);
            _localModelId = modelInfo.ModelId;

            progress?.Report(new FoundryLocalDownloadProgress(
                modelAlias, 100, 0, 0, "ready"));

            _logger.LogInformation(
                "Foundry Local ready. Endpoint={Endpoint}, ModelId={ModelId}",
                _localManager.Endpoint, _localModelId);

            // Now wire up the IChatClient using the local endpoint
            UpdateClient(
                apiKey: _localManager.ApiKey,
                provider: "local",
                endpoint: _localManager.Endpoint.ToString(),
                model: _localModelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Foundry Local");
            throw;
        }
#else
        throw new PlatformNotSupportedException(
            "Foundry Local is not available on this platform.");
#endif
    }

    // ================================================================
    // NEW: Toggle between cloud and local at runtime
    // ================================================================

    public async Task ToggleProviderAsync(bool useLocal)
    {
        if (useLocal)
        {
#if WINDOWS || MACCATALYST
            if (_localManager == null || _localModelId == null)
            {
                // First time — need to download/start the model
                await InitializeLocalAsync();
            }
            else
            {
                // Already initialized, just switch the client
                UpdateClient(
                    apiKey: _localManager.ApiKey,
                    provider: "local",
                    endpoint: _localManager.Endpoint.ToString(),
                    model: _localModelId);
            }
#else
            throw new PlatformNotSupportedException(
                "Foundry Local is not available on this platform.");
#endif
        }
        else
        {
            // Switch back to cloud
            if (_lastCloudApiKey != null && _lastCloudProvider != null)
            {
                UpdateClient(
                    _lastCloudApiKey,
                    _lastCloudProvider,
                    _lastCloudEndpoint,
                    _lastCloudModel ?? "gpt-4o");
            }
            else
            {
                _logger.LogWarning(
                    "No cloud provider configured to switch back to.");
            }
        }
    }

    // ================================================================
    // Helper: wrap IChatClient with logging and tool support
    // ================================================================

    private IChatClient WrapWithToolsAndLogging(IChatClient innerClient)
    {
        var logged = new LoggingChatClient(innerClient, _logger);

        return new ChatClientBuilder(logged)
            .ConfigureOptions(options =>
            {
                options.Tools ??= [];
                options.Tools.Add(
                    AIFunctionFactory.Create(_locationTools.IsNearby));
            })
            .UseFunctionInvocation()
            .Build();
    }

    // ... existing GetMcpToolsAsync and GetResponseWithToolsAsync unchanged ...
}
```

### What this gives us

1. **`InitializeLocalAsync()`** — Call once to download + start the on-device model. Reports progress so you can show a download bar.
2. **`ToggleProviderAsync(bool useLocal)`** — Flip between cloud and local at runtime. Remembers the last cloud config so you can switch back.
3. **`CurrentProvider`** — Observable property the UI can bind to show which mode is active.
4. **`IsLocalAvailable`** — Compile-time check; `false` on iOS/Android so the UI can hide the toggle.

---

## 4. MauiProgram.cs Changes

The `MauiProgram.cs` registration stays simple — `ChatClientService` handles provider initialization internally. The only change is adding the **user's preference for local-first** behavior:

```csharp
// file: MauiProgram.cs

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            // ... existing configuration ...
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        // ── Service registrations ──

        // IChatClientService — the single service that manages all AI providers.
        // It reads preferences internally to decide OpenAI vs Foundry vs Local.
        builder.Services.AddSingleton<IChatClientService, ChatClientService>();

        // All other existing registrations unchanged:
        builder.Services.AddSingleton<ProjectRepository>();
        builder.Services.AddSingleton<TaskRepository>();
        builder.Services.AddSingleton<CategoryRepository>();
        builder.Services.AddSingleton<TagRepository>();
        builder.Services.AddSingleton<SeedDataService>();
        builder.Services.AddSingleton<IUserMemoryStore, SqliteUserMemoryStore>();
        builder.Services.AddSingleton<ModalErrorHandler>();
        builder.Services.AddSingleton<LocationTools>();

        // PageModels
        builder.Services.AddSingleton<MainPageModel>();
        builder.Services.AddSingleton<ProjectListPageModel>();
        builder.Services.AddSingleton<UserProfilePageModel>();
        // ... etc ...

        return builder.Build();
    }
}
```

### Alternative: Register IChatClient Directly via DI

If you prefer the `AddChatClient` extension (registers `IChatClient` directly in DI), you can do it like this. However, Telepathy uses `IChatClientService` for runtime switching, which is the more flexible pattern:

```csharp
// Alternative approach — register IChatClient directly
// (This is simpler but doesn't support runtime provider switching)

// Cloud: Microsoft Foundry
builder.Services.AddChatClient(sp =>
{
    var endpoint = new Uri("https://your-resource.openai.azure.com");
    var credential = new System.ClientModel.ApiKeyCredential("your-key");
    var azureClient = new AzureOpenAIClient(endpoint, credential);
    return azureClient.GetChatClient("gpt-4o").AsIChatClient();
});

// OR Local: Foundry Local (startup is async, so use a factory)
builder.Services.AddChatClient(async sp =>
{
    var manager = await FoundryLocalManager.StartModelAsync("phi-4-mini");
    var modelInfo = await manager.GetModelInfoAsync("phi-4-mini");
    var client = new OpenAIClient(
        new System.ClientModel.ApiKeyCredential(manager.ApiKey),
        new OpenAIClientOptions { Endpoint = manager.Endpoint });
    return client.GetChatClient(modelInfo.ModelId).AsIChatClient();
});
```

---

## 5. ViewModel Usage — Settings Toggle

The `UserProfilePageModel` already manages AI settings. Here's how to add the local/cloud toggle:

```csharp
// file: PageModels/UserProfilePageModel.cs (additions)

public partial class UserProfilePageModel : ObservableObject
{
    private readonly IChatClientService _chatClientService;
    private readonly ILogger<UserProfilePageModel> _logger;

    // ── NEW: Local AI toggle state ──

    [ObservableProperty]
    private bool _useLocalAi = Preferences.Default.Get("use_local_ai", false);

    [ObservableProperty]
    private bool _isLocalAvailable;

    [ObservableProperty]
    private bool _isDownloadingModel;

    [ObservableProperty]
    private double _downloadProgress;

    [ObservableProperty]
    private string _downloadStatus = string.Empty;

    [ObservableProperty]
    private string _currentProviderLabel = "Cloud";

    public UserProfilePageModel(
        IChatClientService chatClientService,
        ICalendarStore calendarStore,
        LocationTools locationTools,
        ILogger<UserProfilePageModel> logger)
    {
        _chatClientService = chatClientService;
        _calendarStore = calendarStore;
        _locationTools = locationTools;
        _logger = logger;

        // Check platform support
        IsLocalAvailable = _chatClientService.IsLocalAvailable;
        CurrentProviderLabel = _chatClientService.CurrentProvider switch
        {
            "local" => "On-Device (Foundry Local)",
            "foundry" => "Cloud (AI Foundry)",
            "openai" => "Cloud (OpenAI)",
            _ => "Not configured"
        };
    }

    /// <summary>
    /// Called when the user flips the "Use Local AI" toggle.
    /// Handles model download on first use and provider switching.
    /// </summary>
    partial void OnUseLocalAiChanged(bool value)
    {
        Preferences.Default.Set("use_local_ai", value);
        _ = SwitchProviderAsync(value);
    }

    private async Task SwitchProviderAsync(bool useLocal)
    {
        try
        {
            if (useLocal)
            {
                IsDownloadingModel = true;
                DownloadStatus = "Starting local AI model...";
                DownloadProgress = 0;

                var progress = new Progress<FoundryLocalDownloadProgress>(p =>
                {
                    // Update UI on main thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DownloadProgress = p.PercentComplete;
                        DownloadStatus = p.Status switch
                        {
                            "downloading" =>
                                $"Downloading model... {p.PercentComplete:F0}%",
                            "extracting" => "Preparing model...",
                            "ready" => "Model ready!",
                            _ => p.Status
                        };
                    });
                });

                await _chatClientService.InitializeLocalAsync(
                    modelAlias: "phi-4-mini",
                    progress: progress);

                CurrentProviderLabel = "On-Device (Foundry Local)";
                DownloadStatus = "Running locally — no internet needed";
            }
            else
            {
                await _chatClientService.ToggleProviderAsync(useLocal: false);
                CurrentProviderLabel = _chatClientService.CurrentProvider switch
                {
                    "foundry" => "Cloud (AI Foundry)",
                    "openai" => "Cloud (OpenAI)",
                    _ => "Cloud"
                };
                DownloadStatus = string.Empty;
            }
        }
        catch (PlatformNotSupportedException)
        {
            UseLocalAi = false;
            await Shell.Current.DisplayAlert(
                "Not Supported",
                "On-device AI is only available on Windows and macOS.",
                "OK");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to switch AI provider");
            UseLocalAi = false;
            await Shell.Current.DisplayAlert(
                "Error",
                $"Failed to start local AI: {ex.Message}",
                "OK");
        }
        finally
        {
            IsDownloadingModel = false;
        }
    }
}
```

### Settings Page XAML (toggle UI)

```xml
<!-- file: Pages/UserProfilePage.xaml (additions inside the settings section) -->

<!-- ============================================ -->
<!-- AI Provider Toggle                           -->
<!-- ============================================ -->
<VerticalStackLayout Spacing="8" Padding="16,8">

    <Label Text="AI Provider"
           Style="{StaticResource SectionHeader}" />

    <Label Text="{Binding CurrentProviderLabel}"
           FontSize="14"
           TextColor="{AppThemeBinding Light={StaticResource Gray600},
                                       Dark={StaticResource Gray300}}" />

    <!-- Only show toggle on platforms that support local AI -->
    <HorizontalStackLayout Spacing="12"
                           IsVisible="{Binding IsLocalAvailable}">
        <Switch IsToggled="{Binding UseLocalAi}"
                IsEnabled="{Binding IsDownloadingModel,
                            Converter={StaticResource InvertedBoolConverter}}" />
        <Label Text="Use on-device AI (Foundry Local)"
               VerticalOptions="Center" />
    </HorizontalStackLayout>

    <!-- Download progress (shown during first-time model download) -->
    <VerticalStackLayout IsVisible="{Binding IsDownloadingModel}"
                         Spacing="4">
        <ProgressBar Progress="{Binding DownloadProgress}"
                     ProgressColor="{StaticResource Primary}" />
        <Label Text="{Binding DownloadStatus}"
               FontSize="12"
               TextColor="{StaticResource Gray500}" />
    </VerticalStackLayout>

    <!-- Platform notice for iOS -->
    <Label Text="On-device AI requires Windows or macOS"
           FontSize="12"
           TextColor="{StaticResource Gray400}"
           IsVisible="{Binding IsLocalAvailable,
                       Converter={StaticResource InvertedBoolConverter}}" />

</VerticalStackLayout>
```

### ProjectDetailPageModel — Zero Changes Needed

This is the beauty of the abstraction. The `ProjectDetailPageModel` calls `_chatClientService.GetClient()` and gets back whatever `IChatClient` is currently active. No changes required:

```csharp
// file: PageModels/ProjectDetailPageModel.cs
// This code is UNCHANGED — it already works with local or cloud

private async Task GetRecommendationsAsync(string projectName)
{
    try
    {
        IsBusy = true;
        BusyTitle = "Getting task recommendations.";

        // GetClient() returns whichever IChatClient is active:
        // - AzureOpenAIClient-backed (cloud) OR
        // - OpenAIClient-backed pointed at localhost (Foundry Local)
        var chatClient = _chatClientService.GetClient();

        var prompt = $"Given a project named '{projectName}', " +
                     $"suggest 3-7 tasks for this project. " +
                     "Respond ONLY with valid JSON in this format, no other text: " +
                     "{\"category\":\"category name\",\"tasks\":[\"task1\",\"task2\"]}";

        // Use non-generic GetResponseAsync and manually extract JSON.
        // Small local models often wrap output in <think> tags, code fences,
        // or preamble text that breaks automatic deserialization.
        var rawResponse = await chatClient.GetResponseAsync(prompt);
        var rawText = rawResponse.Text ?? string.Empty;

        // Clean up model output
        rawText = Regex.Replace(rawText, @"<think>[\s\S]*?</think>\s*", "").Trim();
        rawText = rawText.Replace("```json", "").Replace("```", "").Trim();
        var jsonMatch = Regex.Match(rawText, @"(\{[\s\S]*?\})");
        if (jsonMatch.Success)
            rawText = jsonMatch.Value;

        var response = JsonSerializer.Deserialize<RecommendationResponse>(rawText,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (response != null)
        {
            // Process recommendations...
        }
    }
    catch (InvalidOperationException ex)
    {
        _errorHandler.HandleError(new Exception(
            "AI is not configured. Add an API key in Settings " +
            "or enable on-device AI.", ex));
    }
    finally
    {
        IsBusy = false;
    }
}
```

---

## 6. Key Considerations

### Model Size and First-Run Download UX

Foundry Local models are large (typically **1–3 GB**). The first time a user enables local AI, the model must be downloaded. Handle this gracefully:

```csharp
// Show a bottom sheet or modal explaining the download
if (!Preferences.Default.Get("local_model_downloaded", false))
{
    bool proceed = await Shell.Current.DisplayAlert(
        "Download Required",
        "On-device AI requires downloading a ~2 GB model. " +
        "This only happens once. Continue?",
        "Download", "Cancel");

    if (!proceed) return;
}
```

### Performance Characteristics

| Aspect | Cloud (AI Foundry) | Local (Foundry Local) |
|--------|-------------------|-----------------------|
| **Latency (first token)** | 200–800ms | 1–5s (model load), then 500ms–2s |
| **Throughput** | 30–80 tokens/sec | 5–30 tokens/sec (device dependent) |
| **Model capability** | GPT-4o (frontier) | Phi-4-mini (SLM, good for tasks) |
| **Works offline** | ❌ | ✅ |
| **Cost** | Per-token billing | Free after download |
| **Privacy** | Data sent to cloud | Data stays on device |

### Platform Support Matrix

| Platform | Foundry Local | Acceleration |
|----------|:------------:|:------------:|
| **Windows** | ✅ | DirectML GPU via WinML |
| **macOS (Catalyst)** | ✅ | CPU (Metal support TBD) |
| **iOS** | ❌ | — |
| **Android** | ❌ | — |

### Handling Unsupported Platforms Gracefully

Use `#if` directives and the `IsLocalAvailable` property:

```csharp
// In ViewModels — check before showing local AI options
if (_chatClientService.IsLocalAvailable)
{
    // Show the toggle in UI
}

// In the service — compile-time guards
#if WINDOWS || MACCATALYST
    // Foundry Local code here
#else
    throw new PlatformNotSupportedException(
        "Foundry Local is not available on this platform.");
#endif
```

### Model Selection

Foundry Local supports several models. Choose based on your task:

```csharp
// Good for task generation, classification, structured output
await _chatClientService.InitializeLocalAsync("phi-4-mini");

// Larger model, better quality, slower inference
await _chatClientService.InitializeLocalAsync("phi-4");

// List available models
#if WINDOWS || MACCATALYST
var manager = await FoundryLocalManager.StartModelAsync("phi-4-mini");
var catalog = await manager.ListModelsAsync();
foreach (var model in catalog)
{
    Console.WriteLine($"{model.Alias}: {model.ModelId} ({model.SizeInBytes / 1_000_000}MB)");
}
#endif
```

### Structured Output with Local Models

Small local models often wrap their output in `<think>` reasoning tags, markdown code fences, or preamble text like "Sure, here are the results:". This breaks automatic JSON deserialization. Use manual extraction instead:

```csharp
using System.Text.Json;
using System.Text.RegularExpressions;

public async Task<T?> GetStructuredResponseAsync<T>(
    IChatClient client, string prompt, int maxRetries = 2)
{
    // Be explicit about the format you need
    prompt += " Respond ONLY with valid JSON, no other text.";

    for (int i = 0; i <= maxRetries; i++)
    {
        try
        {
            var response = await client.GetResponseAsync(prompt);
            var text = response.Text ?? string.Empty;

            // Strip <think>...</think> tags (Qwen3, DeepSeek-R1)
            text = Regex.Replace(text, @"<think>[\s\S]*?</think>\s*", "").Trim();

            // Strip markdown code fences
            text = text.Replace("```json", "").Replace("```", "").Trim();

            // Extract first JSON object or array
            var match = Regex.Match(text, @"(\{[\s\S]*?\}|\[[\s\S]*?\])");
            if (match.Success)
                text = match.Value;

            return JsonSerializer.Deserialize<T>(text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException) when (i < maxRetries)
        {
            // Retry with even more explicit instructions
            prompt = $"You MUST respond with ONLY valid JSON. No explanation. {prompt}";
        }
    }
    return default;
}
```

> **Why not `GetResponseAsync<T>`?** The generic extension method from Microsoft.Extensions.AI adds its own system prompt and tries to parse the raw model output as JSON. With cloud models (GPT-4o, etc.) this works reliably. With smaller local models that emit thinking tags or preamble text, it fails. Manual extraction is more resilient.

### Offline Detection — Auto-Fallback

You can automatically switch to local AI when the device goes offline:

```csharp
// file: Services/ConnectivityWatcher.cs

public class ConnectivityWatcher
{
    private readonly IChatClientService _chatClientService;

    public ConnectivityWatcher(IChatClientService chatClientService)
    {
        _chatClientService = chatClientService;
        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (!_chatClientService.IsLocalAvailable) return;

        bool isOffline = e.NetworkAccess != NetworkAccess.Internet;

        if (isOffline && _chatClientService.CurrentProvider != "local")
        {
            // Auto-switch to local when offline
            await _chatClientService.ToggleProviderAsync(useLocal: true);
        }
        else if (!isOffline && Preferences.Default.Get("prefer_cloud", true)
                 && _chatClientService.CurrentProvider == "local")
        {
            // Auto-switch back to cloud when online (if user prefers cloud)
            await _chatClientService.ToggleProviderAsync(useLocal: false);
        }
    }
}
```

Register it in `MauiProgram.cs`:

```csharp
builder.Services.AddSingleton<ConnectivityWatcher>();
```

---

## Summary

| What | Where | Key Change |
|------|-------|------------|
| NuGet packages | `Telepathic.csproj` | Conditional `Microsoft.AI.Foundry.Local` / `.WinML` |
| Provider logic | `ChatClientService.cs` | New `"local"` case + `InitializeLocalAsync` + `ToggleProviderAsync` |
| Settings UI | `UserProfilePageModel.cs` | Toggle switch, download progress, provider label |
| ViewModels | `ProjectDetailPageModel.cs` | **No changes** — same `GetClient()` call works for both |
| Offline fallback | `ConnectivityWatcher.cs` (new) | Auto-switch on network state changes |

The core principle: **one interface, two runtimes, zero ViewModel changes**. The `IChatClient` from `Microsoft.Extensions.AI` makes cloud and on-device models interchangeable, and the `IChatClientService` wrapper handles the lifecycle and switching mechanics.

# .NET MAUI 11 samples

This is the current stable sample set. Samples are copied from `10.0/`, with the existing .NET 11 feature samples preserved. The `9.0/` and `10.0/` folders are unchanged.

## Requirements

- [.NET SDK 11.0.100](https://dotnet.microsoft.com/download/dotnet/11.0) or a later stable .NET 11 SDK, selected by [`global.json`](global.json).
- The .NET MAUI workload and native tooling for the platforms you build.
- Xcode 27.2 for the Apple builds used to validate this migration.
- Minimum deployment targets: Android API 24, iOS 15, and Mac Catalyst 17 (macOS 14). Individual samples can require newer versions.
- Additional service credentials, signing identities, native libraries, or test devices where a sample's README requires them.

For example, on a configured Mac:

```bash
cd 11.0
dotnet build UserInterface/Views/CollectionViewDemos/CollectionViewDemos/CollectionViewDemos.csproj \
  -f net11.0-maccatalyst -p:TargetFrameworks=net11.0-maccatalyst
```

The `TargetFrameworks` override limits restore and build to the selected platform. Omit it to restore all supported platforms. Windows builds require Windows. Device signing, service integration, and Appium/BrowserStack tests require their own setup.

The SDK can also be installed locally in `.dotnet/`; `global.json` searches that directory before the system installation. See [SDK selection](https://learn.microsoft.com/dotnet/core/tools/global-json).

## Package versions

[`Directory.Build.props`](Directory.Build.props) selects stable MAUI **11.0.10** and Microsoft Extensions **11.0.0**. Projects use `$(MauiVersion)` and `$(DotNetVersion)` rather than individual version literals. See [centralized package versions](PACKAGE-VERSIONS.md).

The releases were checked against [.NET release metadata](https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/11.0/releases.json) and the [MAUI](https://www.nuget.org/packages/Microsoft.Maui.Controls/11.0.10) and [Extensions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Debug/11.0.0) package feeds.

## Migration decisions

- **Replaced by CollectionViewDemos:** `UserInterface/Views/ListViewDemos` is not copied because its entire purpose is the deprecated ListView. Every ListViewDemos page has a CollectionView equivalent in [CollectionViewDemos](UserInterface/Views/CollectionViewDemos#coming-from-listviewdemos), which maps each page and adds a context-menu (`ContextFlyout`) replacement for ListView context actions. The original sample remains in [10.0/UserInterface/Views/ListViewDemos](../10.0/UserInterface/Views/ListViewDemos) for reference.
- **Omitted gallery pages:** `EntryCellDemoPage`, `ImageCellDemoPage`, `SwitchCellDemoPage`, `TextCellDemoPage`, `ViewCellDemoPage`, `ListViewDemoPage`, `TableViewFormDemoPage`, and `TableViewMenuDemoPage` under `UserInterface/ControlGallery/ControlGallery/Views/XAML`. These teach obsolete cells or controls; the gallery already has Entry, Image, Switch, Label, layouts, and CollectionView alternatives. Their menu entries and project metadata are removed too.
- **Replaced teaching page:** `XAML/Fundamentals/XamlSamples/Views/ListViewDemoPage` becomes `CollectionViewDemoPage`, retaining the color-list data-binding scenario with view-based templates.
- **Migrated incidental lists:** BehaviorsDemos, DataBindingDemos, ModalNavigation, PlatformIntegrationDemos, XAML Fundamentals, and MarkupExtensions use CollectionView, view templates, and selection-change event arguments. Bounded Grid rows replace unconstrained stacking parents where needed for scrolling.
- **Migrated decoration:** EmployeeDirectory, DragAndDropGesture, and SwipeGesture use Border instead of Frame, with explicit corner shapes, stroke, padding, and shadows. Unused legacy template styles are removed.
- **Migrated APIs:** application startup uses `CreateWindow`; alert, action-sheet, and animation calls use supported async APIs. Puzzle layout animation uses translation, then commits the final AbsoluteLayout bounds. Unused `Animations/Pages/Controls/PickerCell` is removed because it derived from obsolete ViewCell.
- **Migrated media and graphics:** photo/video selection uses `PickPhotosAsync`/`PickVideosAsync`, with a single-selection limit and first-result handling for the existing single-media scenarios. SkiaSharp transformations use the supported matrix factories and instance concatenation APIs.
- **Updated compatibility:** CarouselView's spacing helper references CarouselView, not its former CollectionView base type. BrowserStack.TestAdapter is updated from 0.13.3 to stable 0.53.0 because the old adapter's MonoMod dependency rejects .NET 11; remote test execution still requires BrowserStack setup. Syncfusion.Maui.Toolkit moves to 1.0.12 in ChatClient, ChatClientWithTools, BillingService, PickerDemo, and DeveloperBalance; the 1.0.6 and 1.0.7 Windows resource indexes reference `WebView2Loader.dll` files the packages do not contain, which breaks Windows builds.
- **Updated platform configuration:** the packaging consumer uses the SDK's Android runtime defaults instead of requesting unavailable 32-bit CoreCLR packs. NativeEmbeddingDemo no longer sets the unsupported Mac Catalyst toolbar baseline-separator property.
- **Migrated layout and typography:** expanding StackLayouts use Grid star tracks rather than obsolete `AndExpand` options. Deprecated named font sizes use explicit device-independent sizes (Micro 10, Small 14, Medium 17, Large 22), retaining automatic font scaling.
- **Adopted scenes:** MAUI Apple apps include scene manifests and `MauiUISceneDelegate` subclasses for Xcode 27. Existing native-embedding scene configurations are retained. See [UIKit scene lifecycle migration](https://learn.microsoft.com/dotnet/maui/ios/uikit-scene-lifecycle).
- **Preserved scene-dependent behavior:** EmployeeDirectory initializes its iOS phone-feature service after its scene's root controller exists, uses the supported URL-opening overload, and places calls through PhoneDialer. The gallery still demonstrates real stack layouts, with Grid shown separately for remaining-space allocation.
- **Removed obsolete compilation metadata:** WhatToEat no longer uses the deprecated assembly-level `XamlCompilation` attribute; the current XAML build pipeline handles compilation.
- **Removed dependency:** `Microsoft.Maui.Controls.Compatibility` is no longer shipped in .NET 11 and is not referenced. See [.NET MAUI 11 changes](https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11?view=net-maui-11.0).

No obsolete-warning suppressions are added. Platform-specific APIs still need validation on their own target platforms, and samples that depend on remote services need configured services to exercise those scenarios.

## Migration validation

The migration was checked with .NET SDK 11.0.100, MAUI 11.0.10, and Xcode 27.2 on an Apple Silicon Mac.

- **Compiler audit:** 140 of 141 projects compile for one selected locally supported target each: 112 Mac Catalyst, 21 plain .NET, five iOS, and two Android. The remaining project is the Windows-only NativeEmbeddingDemo WinUI host, which requires Windows. This is not a full native build of every project or every target.
- **Additional platform compiles:** EmployeeDirectory, PlatformIntegrationDemos, SwipeGesture, WeatherTwentyOne, DataBindingDemos, BehaviorsDemos, ControlGallery, VideoDemos, Passkeys.Client, and WordPuzzle each compile for iOS simulator arm64 and Android, adding 20 target checks. Neither compiler audit reports obsolete API diagnostics (`CS0612`, `CS0618`, or `CA1422`).
- **Full native builds:** SwipeGesture, WeatherTwentyOne, WordPuzzle, and DataBindingDemos build for Mac Catalyst; ModalNavigation builds for Android arm64. The packaging library builds and packs for Android, iOS, and Mac Catalyst, and its consumer compiles against that local package.
- **Structural checks:** all 135 intended source projects and six existing destination projects are present; binary assets are unchanged, project/solution references resolve, and all 222 MAUI Apple application platform folders have matching scene manifests and delegates. No changes were made to `9.0/` or `10.0/`.
- **Runtime checks (iOS 27.2 simulator):** XamlSamples launches, and its replacement `CollectionViewDemoPage` renders and scrolls the color list. In CollectionViewDemos, the context menu items page renders, swiping a row shows Favorite and Delete, and Delete removes the item. On Mac Catalyst, CollectionViewDemos launches and the page renders. The right-click `ContextFlyout` could not be triggered through desktop automation, so it is unverified at runtime.
- **Not verified at runtime:** other migrated pages and samples (for example DataBindingDemos selection and WordPuzzle/PhotoPuzzle animations), Android, Windows, physical-device features, external services, and BrowserStack execution.
- **Hosted CI (pull request workflow):** the Windows host builds every changed project, including the NativeEmbeddingDemo WinUI host, the Syncfusion-based samples, and the local-package consumer. The macOS (`xcode-27`) host builds every changed project except the Windows-only WinUI host, which it is configured to skip. CI builds do not run apps.
- **Remaining warnings:** existing nullable/event-handler and XAML binding warnings remain. PointOfSale's Microcharts dependency still produces a legacy MonoAndroid framework warning. The updated Aspire host also reports `ASPIRE010` because the optional CLI bundle is not enabled. These warnings are not suppressed. The dependency advisory fixes and their validation are described below.

### Dependency security updates

The advisory remediation keeps existing dependency families where possible, using patched versions rather than suppressing NuGet audit warnings.

On 2026-11-24, the audit against NuGet.org reports **zero known vulnerable packages**, including transitive dependencies, across all 141 project graphs evaluated on this Mac. `dotnet list package --vulnerable --include-transitive` checks 140 projects. For the Windows-only host, that command fails with `Sequence contains no matching element`; a separate restore with `EnableWindowsTargeting=true`, `NuGetAudit=true`, `NuGetAuditMode=all`, and `NuGetAuditLevel=low` completes without warnings. This covers the frameworks restored on macOS and that Windows host, not every conditional Windows target variant.

| Sample | Package changes | Validation after updating |
| --- | --- | --- |
| DeveloperBalance and TodoSQLite | Pin transitive `SQLitePCLRaw.lib.e_sqlite3` and its Android package from 2.1.10 / 2.1.2 to 2.1.12. | Both apps build for iOS simulator and Mac Catalyst and compile for Android. Each saves a record that survives an iOS app restart. Separate checks of both managed/provider combinations load the patched macOS SQLite 3.53.3 library and pass create/read/update/delete operations. |
| PointOfSale.API | EF Core SqlServer/Tools 7.0.0 to 7.0.16; Microsoft.Identity.Web 2.0.7-preview to 2.17.0; pin Azure.Identity to 1.11.4. This also resolves SqlClient 5.1.4, IdentityModel token packages 7.3.1, and System.Drawing.Common 6.0.0 instead of vulnerable versions. | Builds without warnings and starts/responds to HTTP. Its existing server has no mapped controllers, so the root returns 404; database and identity-provider integration are not exercised. |
| MauiAspire.AppHost | Aspire SDK/Hosting 13.0.0 to 13.6.5; DevTunnels preview to stable 13.6.5; MAUI integration to matching 13.6.5-preview.1.26559.7. Dashboard moves to 13.6.5 and transitive MessagePack from 2.5.192 to 2.5.307. | Builds and starts with isolated ports/secrets. The webapi resource becomes healthy and serves five weather forecasts. Mobile launch commands and authenticated dev tunnels are not exercised. |
| MauiAspire/SampleWebApi | Microsoft.AspNetCore.OpenApi 10.0.0 to 11.0.0, resolving Microsoft.OpenApi 3.11.0 instead of vulnerable 2.0.0. | Builds through the AppHost; `/openapi/v1.json` returns a valid document containing the weather endpoint. |
| NativeEmbeddingDemo.WinUI | Remove the obsolete Windows App SDK 1.6 pin so MAUI selects its required 2.5.1 version. The pin caused a package-downgrade restore error during the audit. | Windows-targeted restore succeeds on macOS; native build and runtime still require Windows. |

`SQLitePCLRaw.bundle_green` has no patched bundle release, so the two SQLite samples retain their existing managed provider configuration and explicitly update the vulnerable native dependencies. On iOS that configuration uses the system SQLite provider; the separate macOS native-binary check exercises the updated bundled SQLite library. The Aspire MAUI integration remains preview because no stable release exists for that package.

A clean advisory audit is not a claim that every sample is production-ready: some retained dependency families are out of support, and platform-specific behavior, remote services, and authenticated flows still need their own validation.

## Sample coverage

The table below accounts for every source project through its owning sample path. A sample is grouped by its nearest README (or project directory when it has none); multi-project samples show their project count. All assets and supporting files are copied with their sample, except the explicitly omitted paths above.

**135 projects in 107 baseline sample groups copied; one ListView-only sample/project replaced by the existing CollectionViewDemos. Five existing .NET 11 feature demos (six projects) retained, for 141 projects in this folder.** The one replaced XAML teaching page is inside a copied sample, not an additional sample.

| Baseline sample path (relative to `10.0/`) | Projects | Decision |
| --- | ---: | --- |
| [AI/ChatClient](AI/ChatClient) | 1 | Copied and retargeted. |
| [AI/ChatClientWithTools](AI/ChatClientWithTools) | 1 | Copied and retargeted. |
| [AgeSignals](AgeSignals) | 2 | Copied and retargeted. |
| [Animations](Animations) | 1 | Copied and retargeted. |
| [Apps/BugSweeper](Apps/BugSweeper) | 1 | Copied and retargeted. |
| [Apps/Calculator](Apps/Calculator) | 1 | Copied and retargeted. |
| [Apps/DeveloperBalance](Apps/DeveloperBalance) | 1 | Copied and retargeted. |
| [Apps/EmployeeDirectory](Apps/EmployeeDirectory) | 2 | Copied; incidental Frames replaced with Borders. |
| [Apps/GameOfLife](Apps/GameOfLife) | 1 | Copied and retargeted. |
| [Apps/PointOfSale](Apps/PointOfSale) | 2 | Copied and retargeted. |
| [Apps/RpnCalculator](Apps/RpnCalculator) | 1 | Copied and retargeted. |
| [Apps/SolitaireEncryption](Apps/SolitaireEncryption) | 2 | Copied and retargeted. |
| [Apps/TipCalc](Apps/TipCalc) | 1 | Copied and retargeted. |
| [Apps/Weather](Apps/Weather) | 1 | Copied and retargeted. |
| [Apps/WeatherTwentyOne](Apps/WeatherTwentyOne) | 1 | Copied and retargeted. |
| [Apps/WhatToEat](Apps/WhatToEat) | 1 | Copied and retargeted. |
| [Apps/WordPuzzle](Apps/WordPuzzle) | 1 | Copied; layout animation replaced with translation. |
| [Beginners-Series/BeginnersTask](Beginners-Series/BeginnersTask) | 1 | Copied and retargeted. |
| [BillingService](BillingService) | 1 | Copied and retargeted. |
| [Data/TodoSQLite](Data/TodoSQLite) | 1 | Copied and retargeted. |
| [Fundamentals/BehaviorsDemos](Fundamentals/BehaviorsDemos) | 1 | Copied; incidental lists migrated to CollectionView. |
| [Fundamentals/ClassHierarchy](Fundamentals/ClassHierarchy) | 1 | Copied and retargeted. |
| [Fundamentals/ContextMenu](Fundamentals/ContextMenu) | 1 | Copied and retargeted. |
| [Fundamentals/ControlTemplateDemos](Fundamentals/ControlTemplateDemos) | 1 | Copied and retargeted. |
| [Fundamentals/DataBindingDemos](Fundamentals/DataBindingDemos) | 1 | Copied; incidental lists migrated to CollectionView. |
| [Fundamentals/DataTemplateDemos](Fundamentals/DataTemplateDemos) | 1 | Copied and retargeted. |
| [Fundamentals/Localization](Fundamentals/Localization) | 1 | Copied and retargeted. |
| [Fundamentals/Shell](Fundamentals/Shell) | 1 | Copied and retargeted. |
| [Fundamentals/Tooltips](Fundamentals/Tooltips) | 1 | Copied and retargeted. |
| [Fundamentals/TriggersDemos](Fundamentals/TriggersDemos) | 1 | Copied and retargeted. |
| [Navigation/FlyoutPageNavigation](Navigation/FlyoutPageNavigation) | 1 | Copied and retargeted. |
| [Navigation/FlyoutPageSample](Navigation/FlyoutPageSample) | 1 | Copied and retargeted. |
| [Navigation/Hierarchical](Navigation/Hierarchical) | 1 | Copied and retargeted. |
| [Navigation/LoginFlow](Navigation/LoginFlow) | 1 | Copied and retargeted. |
| [Navigation/ModalNavigation](Navigation/ModalNavigation) | 1 | Copied; incidental lists migrated to CollectionView. |
| [Navigation/PassingData](Navigation/PassingData) | 1 | Copied and retargeted. |
| [Navigation/Pop-ups](Navigation/Pop-ups) | 1 | Copied and retargeted. |
| [Navigation/ShellFlyoutSample](Navigation/ShellFlyoutSample) | 1 | Copied and retargeted. |
| [Navigation/ShellMixedSample](Navigation/ShellMixedSample) | 1 | Copied and retargeted. |
| [Navigation/ShellTabBarSample](Navigation/ShellTabBarSample) | 1 | Copied and retargeted. |
| [Navigation/TabbedPage](Navigation/TabbedPage) | 1 | Copied and retargeted. |
| [Navigation/TabbedPageDemo](Navigation/TabbedPageDemo) | 1 | Copied and retargeted. |
| [Navigation/TabbedPageSVGIcons](Navigation/TabbedPageSVGIcons) | 1 | Copied and retargeted. |
| [Navigation/TabbedPageWithNavigationPage](Navigation/TabbedPageWithNavigationPage) | 1 | Copied and retargeted. |
| [Navigation/TitleView](Navigation/TitleView) | 1 | Copied and retargeted. |
| [Packaging/NuGetWithMSBuildFiles](Packaging/NuGetWithMSBuildFiles) | 2 | Copied and retargeted. |
| [PlatformIntegration/InvokePlatformCodeDemos](PlatformIntegration/InvokePlatformCodeDemos) | 1 | Copied and retargeted. |
| [PlatformIntegration/LiveActivityDemo](PlatformIntegration/LiveActivityDemo) | 1 | Copied and retargeted. |
| [PlatformIntegration/LocalNotificationsDemo](PlatformIntegration/LocalNotificationsDemo) | 1 | Copied and retargeted. |
| [PlatformIntegration/NativeEmbeddingDemo](PlatformIntegration/NativeEmbeddingDemo) | 6 | Copied and retargeted. |
| [PlatformIntegration/PlatformIntegrationDemos](PlatformIntegration/PlatformIntegrationDemos) | 1 | Copied; incidental lists migrated to CollectionView. |
| [Sensors/TiltMaze](Sensors/TiltMaze) | 1 | Copied and retargeted. |
| [SkiaSharp/CatClock](SkiaSharp/CatClock) | 1 | Copied and retargeted. |
| [SkiaSharp/MandelbrotAnimation](SkiaSharp/MandelbrotAnimation) | 1 | Copied and retargeted. |
| [SkiaSharp/PhotoPuzzle](SkiaSharp/PhotoPuzzle) | 1 | Copied; layout animation replaced with translation. |
| [SkiaSharp/SkiaSharpDemos](SkiaSharp/SkiaSharpDemos) | 1 | Copied and retargeted. |
| [SkiaSharp/SpinPaint](SkiaSharp/SpinPaint) | 1 | Copied and retargeted. |
| [Tutorials/ConvertToMvvm/code](Tutorials/ConvertToMvvm/code) | 1 | Copied and retargeted. |
| [Tutorials/CreateNetMauiApp/code](Tutorials/CreateNetMauiApp/code) | 1 | Copied and retargeted. |
| [UITesting/BasicAppiumNunitSample](UITesting/BasicAppiumNunitSample) | 6 | Copied and retargeted. |
| [UITesting/BrowserStackAppiumMaui/BasicAppiumNunitSample](UITesting/BrowserStackAppiumMaui/BasicAppiumNunitSample) | 6 | Copied and retargeted. |
| [UserInterface/BrushesDemos](UserInterface/BrushesDemos) | 1 | Copied and retargeted. |
| [UserInterface/ControlGallery](UserInterface/ControlGallery) | 1 | Copied; obsolete-only pages omitted (listed above). |
| [UserInterface/Handlers/CreateHandlerDemo](UserInterface/Handlers/CreateHandlerDemo) | 1 | Copied and retargeted. |
| [UserInterface/Handlers/CustomizeHandlersDemo](UserInterface/Handlers/CustomizeHandlersDemo) | 1 | Copied and retargeted. |
| [UserInterface/Handlers/WindowButtonsHandlerDemo](UserInterface/Handlers/WindowButtonsHandlerDemo) | 1 | Copied and retargeted. |
| [UserInterface/HyperlinkDemo](UserInterface/HyperlinkDemo) | 1 | Copied and retargeted. |
| [UserInterface/Layouts/AbsoluteLayoutDemos](UserInterface/Layouts/AbsoluteLayoutDemos) | 1 | Copied and retargeted. |
| [UserInterface/Layouts/BindableLayoutDemos](UserInterface/Layouts/BindableLayoutDemos) | 1 | Copied and retargeted. |
| [UserInterface/Layouts/CustomLayoutDemos](UserInterface/Layouts/CustomLayoutDemos) | 1 | Copied and retargeted. |
| [UserInterface/Layouts/FlexLayoutDemos](UserInterface/Layouts/FlexLayoutDemos) | 1 | Copied and retargeted. |
| [UserInterface/Layouts/GridDemos](UserInterface/Layouts/GridDemos) | 1 | Copied and retargeted. |
| [UserInterface/Layouts/StackLayoutDemos](UserInterface/Layouts/StackLayoutDemos) | 1 | Copied and retargeted. |
| [UserInterface/Material3Demo/HealthProfile](UserInterface/Material3Demo/HealthProfile) | 1 | Copied and retargeted. |
| [UserInterface/PickerDemo](UserInterface/PickerDemo) | 1 | Copied and retargeted. |
| [UserInterface/ScaleAndRotate](UserInterface/ScaleAndRotate) | 1 | Copied and retargeted. |
| [UserInterface/SystemThemesDemo](UserInterface/SystemThemesDemo) | 1 | Copied and retargeted. |
| [UserInterface/ThemingDemo](UserInterface/ThemingDemo) | 1 | Copied and retargeted. |
| [UserInterface/Views/CarouselViewDemos](UserInterface/Views/CarouselViewDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/CheckBoxDemos](UserInterface/Views/CheckBoxDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/CollectionViewDemos](UserInterface/Views/CollectionViewDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/ContentViewDemo](UserInterface/Views/ContentViewDemo) | 1 | Copied and retargeted. |
| [UserInterface/Views/GraphicsViewDemos](UserInterface/Views/GraphicsViewDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/HybridWebViewDemo](UserInterface/Views/HybridWebViewDemo) | 1 | Copied and retargeted. |
| [UserInterface/Views/IndicatorViewDemos](UserInterface/Views/IndicatorViewDemos) | 1 | Copied and retargeted. |
| `UserInterface/Views/ListViewDemos` | 1 | Not copied: deprecated-only. Replaced by [CollectionViewDemos](UserInterface/Views/CollectionViewDemos#coming-from-listviewdemos); see the page mapping there. |
| [UserInterface/Views/Map](UserInterface/Views/Map) | 1 | Copied and retargeted. |
| [UserInterface/Views/RadioButtonDemos](UserInterface/Views/RadioButtonDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/RefreshViewDemo](UserInterface/Views/RefreshViewDemo) | 1 | Copied and retargeted. |
| [UserInterface/Views/ScrollViewDemos](UserInterface/Views/ScrollViewDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/ShapesDemos](UserInterface/Views/ShapesDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/SwipeViewDemos](UserInterface/Views/SwipeViewDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/SwitchDemos](UserInterface/Views/SwitchDemos) | 1 | Copied and retargeted. |
| [UserInterface/Views/TitleBarDemo](UserInterface/Views/TitleBarDemo) | 1 | Copied and retargeted. |
| [UserInterface/Views/TwoPaneView](UserInterface/Views/TwoPaneView) | 1 | Copied and retargeted. |
| [UserInterface/WorkingWithColors](UserInterface/WorkingWithColors) | 1 | Copied and retargeted. |
| [UserInterface/WorkingWithFiles](UserInterface/WorkingWithFiles) | 1 | Copied and retargeted. |
| [UserInterface/WorkingWithFonts](UserInterface/WorkingWithFonts) | 1 | Copied and retargeted. |
| [UserInterface/WorkingWithGestures/DragAndDropGesture](UserInterface/WorkingWithGestures/DragAndDropGesture) | 1 | Copied; incidental Frames replaced with Borders. |
| [UserInterface/WorkingWithGestures/PanGesture](UserInterface/WorkingWithGestures/PanGesture) | 1 | Copied and retargeted. |
| [UserInterface/WorkingWithGestures/PinchGesture](UserInterface/WorkingWithGestures/PinchGesture) | 1 | Copied and retargeted. |
| [UserInterface/WorkingWithGestures/SwipeGesture](UserInterface/WorkingWithGestures/SwipeGesture) | 1 | Copied; incidental Frames replaced with Borders. |
| [UserInterface/WorkingWithGestures/TapGesture](UserInterface/WorkingWithGestures/TapGesture) | 1 | Copied and retargeted. |
| [WebServices/MauiAspire](WebServices/MauiAspire) | 7 | Copied and retargeted. |
| [WebServices/PushNotificationsDemo](WebServices/PushNotificationsDemo) | 2 | Copied and retargeted. |
| [WebServices/TodoREST](WebServices/TodoREST) | 2 | Copied and retargeted. |
| [XAML/Fundamentals](XAML/Fundamentals) | 1 | Copied; list teaching page replaced with CollectionViewDemoPage. |
| [XAML/MarkupExtensions](XAML/MarkupExtensions) | 1 | Copied; incidental lists migrated to CollectionView. |

| Existing .NET 11 feature demo | Projects | Decision |
| --- | ---: | --- |
| [Shell route templates](Navigation/ShellRouteTemplates) | 1 | Retained; stable packages and Apple scene setup. |
| [Passkeys](PlatformIntegration/Passkeys) | 2 | Client and server retained; stable packages and Apple scene setup. |
| [Long-press gesture](UserInterface/Gestures/LongPressGesture) | 1 | Retained; stable packages and Apple scene setup. |
| [Style invalidation](UserInterface/Styles/InvalidateStyleDemo) | 1 | Retained; stable packages and Apple scene setup. |
| [Map clustering](UserInterface/Views/Map/MapClustering) | 1 | Retained alongside the copied MapDemo; existing shared README preserved. |

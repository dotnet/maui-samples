---
name: .NET MAUI - Live Activity
description: "A .NET MAUI sample demonstrating an iOS Live Activity implemented with a Swift bridge (ActivityKit) and a WidgetKit extension."
page_type: sample
languages:
- csharp
products:
- dotnet-maui
urlFragment: platformintegration-live-activity
---

.NET MAUI + iOS Live Activity (Manual Xcode Integration)
================================================================

This sample shows how a .NET MAUI app (`LiveActivityDemo`) starts, updates, and ends an iOS **Live Activity**. Because **ActivityKit** and **WidgetKit** are Swift-only, we integrate native Swift code through:

1. A small Swift bridge compiled as an **XCFramework** (P/Invoke surface for C#).
2. A **Widget Extension (.appex)** implemented in SwiftUI that renders the Live Activity / Dynamic Island UI.

Everything below explains the Xcode workflow that produced the native artifacts used in this sample.

## 1. Folder & Component Overview

From inside `10.0/PlatformIntegration/LiveActivityDemo/` you will find:

```text
LiveActivityDemo/                # .NET MAUI project root
  LiveActivityDemo.csproj        # Includes conditional bundling of PlugIns for simulator/device
  App.xaml / MainPage.xaml       # UI with Start / Update / End buttons
  Platforms/iOS/
    Info.plist                   # Has NSSupportsLiveActivities keys
    LiveActivityBridge.xcframework/   # Built Swift bridge framework (already universal for sim/device)
    PlugIns/                     # Drop your built widget .appex here
      (iphonesimulator/)*        # Optional subfolder if you separate simulator appex
  Resources/                     # Fonts, images, splash, styles
native/
  LiveActivityBridge/            # Swift Package (sources for bridge that becomes the XCFramework)
    Package.swift
    Sources/LiveActivityBridge/
  LiveActivityWidget/            # Swift sources for the Widget Extension UI (copied into an Xcode-created target)
    Sources/LiveActivityWidget/
  LiveActivityAppHost/           # Optional sample SwiftUI host app (not required by MAUI build)
```

Key ideas:

* The **MAUI app** owns runtime logic and calls exported C functions (e.g., `LA_Start`, `LA_Update`, `LA_End`) defined in the Swift bridge.
* The **Swift bridge** isolates ActivityKit usage and exposes only C-compatible entry points.
* The **Widget Extension** (WidgetKit) uses the same attributes & state model to render Lock Screen / Dynamic Island content.
* The **.appex** bundle must end up at `LiveActivityDemo.app/PlugIns/<YourWidget>.appex` inside the final iOS app bundle.

---

## 2. How It Works (Lifecycle Flow)

1. User taps **Start** in the MAUI UI.
2. C# executes a P/Invoke into the Swift bridge which calls `Activity.request()` to start a Live Activity with initial attributes/state.
3. WidgetKit renders the Live Activity UI (Lock Screen + Dynamic Island regions) using the context provided.
4. **Update** button triggers another bridge call that sends new state via `activity.update(using:)`.
5. **End** button calls a bridge function that ends the activity (`activity.end`) optionally with a final state.

The MAUI layer never talks directly to the Widget extension—communication occurs implicitly through the system-managed Live Activity identified by its attributes.

---

## 3. One-Time Manual Setup in Xcode

### 3.1 Build the Swift Bridge (XCFramework)

1. Open `native/LiveActivityBridge` (Swift Package) in Xcode.
2. Product → Build (for both a simulator and a device destination at least once so Xcode caches slices).
3. (Optional universal XCFramework) Archive with: Product → Archive (for Any iOS Device) if you plan to distribute. For a local sample, the simple build output is usually enough; the committed `LiveActivityBridge.xcframework` already lives under `Platforms/iOS/`.
4. Copy the resulting `LiveActivityBridge.xcframework` into: `LiveActivityDemo/Platforms/iOS/` (replacing the existing one if updating).

The project file already contains:

```xml
<ItemGroup Condition="Exists('Platforms/iOS/LiveActivityBridge.xcframework')">
  <NativeReference Include="Platforms/iOS/LiveActivityBridge.xcframework">
    <Kind>Framework</Kind>
    <ForceLoad>true</ForceLoad>
  </NativeReference>
</ItemGroup>
```

So it will be linked automatically when present.

### 3.2 Create the Widget Extension Target

1. In Xcode, open (or create a workspace containing) your native sources (you can just open the `native/` folder).
2. File → New → Target… → **Widget Extension** → Check **Include Live Activity**.
3. Name it (e.g.) `LiveActivityWidget` (must use a bundle identifier that starts with the MAUI app id: `com.simplyprofound.maui.liveactivity.`).
4. In the new target’s Build Settings / General, add a dependency on the Swift package(s) containing your shared attributes & bridge (e.g. add the `LiveActivityBridge` package; or if you separate model code, add that specific package).
5. Replace the template Swift files with those under: `native/LiveActivityWidget/Sources/LiveActivityWidget/`.

### 3.3 Ensure Attributes & State Types Match

Both the bridge and the widget must refer to the *same* ActivityKit attributes type (e.g., a `DemoAttributes` struct). Any mismatch means updates won’t flow.

### 3.4 Build the Widget

1. Select the widget scheme.
2. Choose a destination:

* Any iOS Simulator (for testing in simulator)
* Any iOS Device (for physical device)

3. Product → Build.
4. In the Product group, right‑click `LiveActivityWidget.appex` → Show in Finder.

### 3.5 Place the `.appex` Into the MAUI Project

Copy the widget extension bundle **itself** (the `.appex` directory) into:

```
LiveActivityDemo/Platforms/iOS/PlugIns/
```

Simulator and device appex binaries are not interchangeable. If you build both, you can choose either strategy:

* Overwrite the same path depending on what you are about to run.
* OR create a subfolder for simulator: `LiveActivityDemo/Platforms/iOS/PlugIns/iphonesimulator/LiveActivityWidget.appex` (the project file has a conditional include that looks for an `iphonesimulator` path when building with an iOS simulator `RuntimeIdentifier`).

### 3.6 Info.plist Settings (Already Present)

The MAUI app’s `Platforms/iOS/Info.plist` must contain:

* `NSSupportsLiveActivities` = true
* (Optional) `NSSupportsLiveActivitiesFrequentUpdates` = true for more frequent refresh windows.

Push-based Live Activity updates (via APNs) require additional entitlements not shown here; this sample uses local (in-app) updates only.

---

## 4. Building & Running the MAUI App

From the repository root or the sample folder, build with the appropriate TFM and runtime:

```bash
dotnet build -t:Run -f net10.0-ios -r iossimulator-arm64  # Simulator
dotnet build -t:Run -f net10.0-ios -r ios-arm64           # Device (adjust runtime if needed)
```

Buttons on the main page:

* Start – creates a new Live Activity.
* Update – pushes updated progress/state.
* End – terminates the Live Activity.

If deployment succeeds, lock the device / simulator to view the Live Activity on the Lock Screen (and Dynamic Island on supported hardware).

---

## 5. Simulator vs Device Bundling

The project file conditionally bundles either:

* `Platforms/iOS/PlugIns/iphonesimulator/**` when the `RuntimeIdentifier` starts with `iossimulator`.
* Otherwise `Platforms/iOS/PlugIns/**`.

That lets you optionally keep a dedicated simulator build of the widget separate from a device build. If you only manage one appex at a time, you can ignore the `iphonesimulator` subfolder and just copy the correct flavor before each run.

---

## 6. Troubleshooting

| Symptom | Likely Cause | Fix |
|---------|--------------|-----|
| No Live Activity appears | Wrong appex location | Ensure `.../LiveActivityDemo.app/PlugIns/LiveActivityWidget.appex` exists in the built app bundle. |
| Live Activity starts but widget UI blank | Attributes type mismatch | Verify the attributes struct name & module are identical in bridge & widget extension. |
| Build succeeds but runtime fails to load widget | Using simulator appex on device (or vice versa) | Rebuild the widget for the correct destination and recopy. |
| Multiple simultaneous activities unexpected | Start called repeatedly without End | Add guard logic in C# or bridge to track active activity. |
| Updates lag / infrequent | System throttling; FrequentUpdates entitlement missing | Add `NSSupportsLiveActivitiesFrequentUpdates` if appropriate. |

Additional debugging tips:

* Use Console.app (macOS) filtering by your bundle id parts (`LiveActivityWidget` or main app id) to inspect ActivityKit / WidgetKit logs.
* Inspect the built app bundle under `bin/Debug/net10.0-ios/<rid>/` to confirm the PlugIns layout.
* Add temporary logging inside the Swift bridge functions to verify calls from C#.

---

## 7. Extending the Sample

Ideas to explore next:

* Add push-based Live Activity updates (APNs) and handle token registration.
* Include a status query function (e.g., `LA_Status`) to enumerate active activities for diagnostics.
* Animate progress automatically with a background timer updating via the bridge.
* Expand the Dynamic Island UI regions (compact / minimal) with meaningful icons.

---

## 8. Recap

1. Build / update the Swift bridge → copy XCFramework into `Platforms/iOS/`.
2. Create and build the Widget Extension in Xcode.
3. Copy the `.appex` into `Platforms/iOS/PlugIns/` (or the simulator subfolder).
4. Build & run the MAUI app; use the UI buttons to drive the Live Activity.

This process keeps each moving part explicit so you can adapt it to more complex scenarios (multiple activities, richer state models, or APNs integration).

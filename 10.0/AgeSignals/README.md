# Age Signals - Cross-Platform Age Verification Sample

A .NET MAUI application demonstrating age verification APIs across Android, iOS, and Windows platforms. This sample shows how to implement platform-specific age verification using Google Play Age Signals, Apple Declared Age Range, and Windows Age Consent APIs.

## Overview

This sample demonstrates:
- Cross-platform age verification with platform-specific implementations
- Unified interface design for multiple age verification APIs
- Swift-to-.NET binding for iOS Declared Age Range API
- Proper error handling and user experience patterns
- Platform-specific service registration and dependency injection

## Platform Support

| Platform | API | Min Version | Status |
|----------|-----|-------------|--------|
| **Android** | Google Play Age Signals | API 23 (Android 6.0+) | Implemented |
| **iOS** | Declared Age Range | iOS 26.0+ | Implemented |
| **Windows** | Age Consent API | Windows 11 Build 22000+ | Implemented |

**Note**: Mac Catalyst is **not supported**. Apple's DeclaredAgeRange framework is unavailable for Mac Catalyst targets in current Xcode versions.

## Requirements

### Development Environment
- **.NET 10.0 SDK** (RC2 or later)
- **Visual Studio 2022 17.13+** or **VS Code** with .NET MAUI extension
- **macOS** (required for iOS development)

### Platform-Specific Requirements

**Android:**
- Android SDK with API 23+ (Android 6.0+)
- Google Play Services on target device
- NuGet package: `Xamarin.Google.Android.Play.Age.Signals` v0.0.1-beta02

**iOS:**
- **Xcode 16.0+** (for iOS 26.0+ SDK)
- **macOS** for building
- **Physical iOS device** (iOS 26.0+) - API not available in simulator
- Declared Age Range entitlement: `com.apple.developer.declared-age-range`

**Windows:**
- Windows 11 Build 22000 or later
- UserAccountInformation capability

## Project Structure

```
AgeSignals/
├── AgeSignals.sln                                # Solution file
├── README.md                                     # This file
│
├── AgeSignals/                                   # Main MAUI application
│   ├── AgeSignals.csproj                         # Project file with multi-targeting
│   ├── MauiProgram.cs                            # App initialization and DI setup
│   ├── MainPage.xaml                             # UI
│   ├── MainPage.xaml.cs                          # UI logic
│   │
│   ├── Models/                                   # Data models
│   │   ├── AgeVerificationMethod.cs              # Enum of verification methods
│   │   ├── AgeVerificationRequest.cs             # Request model
│   │   └── AgeVerificationResult.cs              # Result model
│   │
│   ├── Services/                                 # Age verification services
│   │   ├── IAgeSignalService.cs                  # Unified interface
│   │   ├── AgeSignalService.Android.cs           # Android implementation
│   │   ├── AgeSignalService.iOS.cs               # iOS implementation
│   │   └── AgeSignalService.Windows.cs           # Windows implementation
│   │
│   ├── Platforms/                                # Platform-specific code
│   │   ├── Android/
│   │   │   └── AndroidManifest.xml               # Android manifest
│   │   ├── iOS/
│   │   │   ├── Entitlements.plist                # iOS entitlements
│   │   │   └── Info.plist                        # iOS app info
│   │   └── Windows/
│   │       └── Package.appxmanifest              # Windows manifest
│   │
│   └── Resources/                                # App resources
│
└── DeclaredAgeRangeWrapperBinding.iOS/           # iOS Swift binding project (REQUIRED for iOS)
    ├── DeclaredAgeRangeWrapperBinding.iOS.csproj # Binding project
    ├── ApiDefinition.cs                          # Objective-C binding definitions
    ├── StructsAndEnums.cs                        # Enums and structures
    └── DeclaredAgeRangeWrapper.xcframework/      # Swift XCFramework (16 files, 420 KB)
        ├── Info.plist                            # Framework metadata
        ├── ios-arm64/                            # Physical device slice
        └── ios-arm64_x86_64-simulator/           # Simulator slice (Intel + Apple Silicon)

**Why the Binding Project is Required:**

Apple's DeclaredAgeRange API is **Swift-only** and cannot be called directly from C#/.NET. The binding project bridges this gap:

1. **Swift API** (Apple) → Only accessible from Swift code
2. **Swift Wrapper** (DeclaredAgeRangeWrapper.swift) → Makes API Objective-C compatible using `@objc` attributes
3. **XCFramework** → Packages Swift wrapper for multiple architectures (device + simulator)
4. **Objective-C Binding** (ApiDefinition.cs) → Generated with Objective Sharpie, maps Objective-C to C#
5. **C# Code** (AgeSignalService.iOS.cs) → Calls the binding like any .NET API

Without this binding chain, .NET MAUI cannot access the API at all.

**Reference Implementation:** [dalexsoto/DeclaredAgeRangeSample](https://github.com/dalexsoto/DeclaredAgeRangeSample)
```

## Building the Project

### Prerequisites Check

1. Verify .NET SDK installation:
   ```bash
   dotnet --version  # Should be 10.0.100-rc.2 or later
   ```

2. Verify workload installation:
   ```bash
   dotnet workload list
   # Should show: maui, ios, android, windows
   ```

### Android

Build and run Android version:

```bash
# Build only
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-android

# Build and run
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-android -t:Run
```

**Testing Notes:**
- Works in Android emulator, but Age Signals API returns "Not yet implemented"
- Full functionality requires physical device with Google Play Services
- API availability depends on device jurisdiction

### Windows

Build and run Windows version (requires Windows machine):

```bash
# Build only
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-windows10.0.22000.0

# Build and run
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-windows10.0.22000.0 -t:Run
```

**Testing Notes:**
- Age consent enforcement depends on region (EU, US, UK, Korea)
- Outside enforced regions, API returns "NotEnforced" status

### iOS

**IMPORTANT**: iOS requires building the Swift XCFramework first.

#### Step 1: Build Swift XCFramework (Required - One-time setup)

The iOS implementation requires a Swift XCFramework that bridges Apple's Declared Age Range API to Objective-C/C#.

**Why This Step is Necessary:**

Apple's DeclaredAgeRange API is Swift-only and inaccessible from C#. The XCFramework contains a Swift wrapper that:
- Exposes the Swift API with `@objc` compatibility attributes
- Provides an Objective-C-callable interface
- Enables .NET bindings through Objective Sharpie

Without the XCFramework, the .NET binding project cannot compile, and iOS builds will fail.

**Requirements:**
- macOS with Xcode 16.0+
- Command Line Tools installed

**Note**: This project does NOT include the XCFramework source or build scripts. You need to:

1. Obtain or create the Swift wrapper project (`DeclaredAgeRangeWrapper`)
2. Build the XCFramework using xcodebuild:
   ```bash
   # Example build process (adapt to your Swift project structure)
   xcodebuild archive \
     -project DeclaredAgeRangeWrapper.xcodeproj \
     -scheme DeclaredAgeRangeWrapper \
     -destination 'generic/platform=iOS' \
     -archivePath build/ios

   xcodebuild archive \
     -project DeclaredAgeRangeWrapper.xcodeproj \
     -scheme DeclaredAgeRangeWrapper \
     -destination 'generic/platform=iOS Simulator' \
     -archivePath build/ios-simulator

   xcodebuild -create-xcframework \
     -framework build/ios.xcarchive/Products/Library/Frameworks/DeclaredAgeRangeWrapper.framework \
     -framework build/ios-simulator.xcarchive/Products/Library/Frameworks/DeclaredAgeRangeWrapper.framework \
     -output DeclaredAgeRangeWrapper.xcframework
   ```

3. Copy the built `DeclaredAgeRangeWrapper.xcframework` to:
   ```
   DeclaredAgeRangeWrapperBinding.iOS/DeclaredAgeRangeWrapper.xcframework/
   ```

**Reference**: See [dalexsoto/DeclaredAgeRangeSample](https://github.com/dalexsoto/DeclaredAgeRangeSample) for complete Swift wrapper implementation.

#### Step 2: Build iOS App

Once the XCFramework is in place:

```bash
# Build only
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-ios

# Build for device (requires provisioning)
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-ios -r ios-arm64
```

**Testing Notes:**
- **Simulator**: API will return error "The operation couldn't be completed" (expected - Apple blocks API in simulator)
- **Physical Device**: Requires:
  - iOS 26.0+ device
  - Provisioning profile with `com.apple.developer.declared-age-range` entitlement
  - Age configured in Family Sharing settings on device

## Platform-Specific Implementation Details

### Android: Google Play Age Signals API

**API Package**: `Xamarin.Google.Android.Play.Age.Signals` v0.0.1-beta02 (Beta)

**Implementation**: `AgeSignals/Services/AgeSignalService.Android.cs`

**Key Classes:**
- `AgeSignalsManager` - Main entry point
- `AgeSignalsRequest` - Request builder
- `AgeSignalsResult` - Response with user status and age range

**User Status Types:**
- `VERIFIED` (1): User is 18+
- `SUPERVISED` (2): Supervised account with age range
- `SUPERVISED_APPROVAL_PENDING` (3): Awaiting guardian approval
- `SUPERVISED_APPROVAL_DENIED` (4): Guardian denied
- `UNKNOWN` (0): Age information unavailable

**Limitations:**
- Requires Google Play Store installation
- Not available in all jurisdictions
- Emulator support limited

### iOS: Declared Age Range API

**Framework**: `DeclaredAgeRange` (iOS 26.0+)

**Implementation**: `AgeSignals/Services/AgeSignalService.iOS.cs`

**Binding Project**: `DeclaredAgeRangeWrapperBinding.iOS/` (**Required** - see explanation below)

**Why Binding Project is Necessary:**

Apple's DeclaredAgeRange API is **Swift-only**. .NET cannot directly call Swift code. The binding project solves this through a multi-layer bridge:

**Architecture:**
```
Swift API → Swift Wrapper → XCFramework → Objective-C Binding → C# Binding → MAUI App
     ↑            ↑              ↑                ↑                  ↑            ↑
   Apple      @objc attrs   Multi-arch      Obj Sharpie         .NET       Your Code
   Native     Make Swift    Package for     Generated         Bindings
              Obj-C         Device+Sim      C# Interop
              Compatible
```

Without this binding chain, .NET MAUI apps cannot access the API. This complexity is unavoidable due to Swift-only restriction.

**Key Types:**
- `DeclaredAgeRangeBridge` - Static method for requesting age range
- `MyAgeRangeResponse` - Response wrapper
- `MyAgeRange` - Age range data (lower/upper bounds, declaration type)

**Response Types:**
- `Sharing`: User agreed to share age range
- `DeclinedSharing`: User declined

**Declaration Types:**
- `SelfDeclared`: User declared their own age
- `GuardianDeclared`: Guardian declared user's age
- `Unknown`: Declaration method unknown

**Limitations:**
- **Physical device only** - API not available in simulator (Apple restriction)
- Requires Family Sharing configuration on device
- Entitlement required: `com.apple.developer.declared-age-range`
- **Mac Catalyst not supported** - DeclaredAgeRange module unavailable for Catalyst targets

### Windows: Age Consent API

**API**: `Windows.System.User.CheckUserAgeConsentGroupAsync`

**Implementation**: `AgeSignals/Services/AgeSignalService.Windows.cs`

**Key Types:**
- `UserAgeConsentGroup`: Child, Minor, Adult
- `UserAgeConsentResult`: Included, NotIncluded, Unknown, NotEnforced, Ambiguous

**Age Groups:**
- **Adult**: 18+ years
- **Minor**: Varies by region
- **Child**: Under age of digital consent

**Result Types:**
- `Included`: User is in the specified group
- `NotIncluded`: User is not in the group
- `Unknown`: Age not configured
- `NotEnforced`: Region doesn't enforce age consent
- `Ambiguous`: Obsolete group type

**Limitations:**
- Only enforced in specific regions (EU, US, UK, Korea)
- Returns `NotEnforced` in other regions

## Architecture & Design Patterns

### Unified Interface

All platforms implement `IAgeSignalService`:

```csharp
public interface IAgeSignalService
{
    bool IsSupported();
    string GetPlatformName();
    Task<AgeVerificationResult> RequestAgeVerificationAsync(
        int minimumAge = 13, 
        object? platformContext = null);
    Task<AgeVerificationResult> RequestAgeVerificationAsync(
        AgeVerificationRequest request);
}
```

### Platform-Specific Files

The project uses `.Android.cs`, `.iOS.cs`, and `.Windows.cs` file naming convention with conditional compilation in `.csproj`:

```xml
<ItemGroup>
  <Compile Include="Services/AgeSignalService.iOS.cs" 
           Condition="$(TargetFramework.Contains('ios'))" />
  <Compile Include="Services/AgeSignalService.Android.cs" 
           Condition="$(TargetFramework.Contains('android'))" />
  <Compile Include="Services/AgeSignalService.Windows.cs" 
           Condition="$(TargetFramework.Contains('windows'))" />
</ItemGroup>
```

**Important**: Platform-specific files should NOT contain `#if IOS`, `#if ANDROID`, etc. The file itself IS the platform implementation.

### Dependency Injection

Services are registered in `MauiProgram.cs`:

```csharp
#if ANDROID
builder.Services.AddSingleton<IAgeSignalService, AgeSignalService>();
#elif IOS
builder.Services.AddSingleton<IAgeSignalService, AgeSignalService>();
#elif WINDOWS
builder.Services.AddSingleton<IAgeSignalService, AgeSignalService>();
#endif
```

## Testing Considerations

### Android Testing
- ✅ **Emulator**: Builds and runs, API returns expected error
- ✅ **Physical Device**: Full functionality (requires Google Play Services)
- **Limitation**: API behavior varies by device jurisdiction

### iOS Testing
- ❌ **Simulator**: API returns "operation couldn't be completed" (expected - [documented by Apple sample](https://github.com/dalexsoto/DeclaredAgeRangeSample#step-3-build-and-run-the-sample-app))
- ✅ **Physical Device**: Full functionality (requires iOS 26.0+, proper entitlements, Family Sharing setup)
- **Limitation**: Requires proper provisioning and device configuration

### Windows Testing
- ✅ **Local Machine**: Full API functionality
- **Limitation**: Returns "NotEnforced" outside enforced regions
- **Regional Testing**: Cannot simulate different regions

## Troubleshooting

### Windows: iOS binding project shows as "unloaded" or causes build errors

**Cause**: The iOS binding project is macOS-only and cannot load on Windows

**Solution**: This is expected behavior. The project is configured to:
- Only load the iOS binding project on macOS (`$([MSBuild]::IsOSPlatform('osx'))`)
- Skip iOS target when building on Windows
- You can still build Android and Windows targets on Windows without issues

**To build on Windows**:
```bash
# Build Android only
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-android

# Build Windows only
dotnet build AgeSignals/AgeSignals.csproj -f net10.0-windows10.0.22000.0
```

The iOS binding project will appear unloaded in Visual Studio on Windows - this is normal and expected.

### iOS: "DeclaredAgeRangeWrapper could not be found"

**Cause**: XCFramework not built or not in correct location

**Solution**:
1. Build the Swift XCFramework (see "Building the Project > iOS > Step 1")
2. Verify file exists: `DeclaredAgeRangeWrapperBinding.iOS/DeclaredAgeRangeWrapper.xcframework/Info.plist`
3. Clean and rebuild: `dotnet clean && dotnet build -f net10.0-ios`

### Android: Build errors with Google Play Age Signals

**Cause**: Beta NuGet package compatibility issues

**Solution**:
- Ensure .NET 10.0 RC2 or later
- Verify package version: `Xamarin.Google.Android.Play.Age.Signals` v0.0.1-beta02
- Clean NuGet cache: `dotnet nuget locals all --clear`

**Cross-Platform Development**:
- **On macOS**: Build Android and iOS targets
- **On Windows**: Build Android and Windows targets  
- iOS binding project will be unloaded on Windows (expected)

## Learn More

### Official Documentation
- [Google Play Age Signals](https://developer.android.com/guide/playcore/age-verification)
- [Apple Declared Age Range](https://developer.apple.com/documentation/declaredagerange/)
- [WWDC 2024: Declared Age Range](https://developer.apple.com/videos/play/wwdc2024/10178/)
- [Windows Age Consent API](https://learn.microsoft.com/en-us/uwp/api/windows.system.user.checkuserageconsentgroupasync)
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)

### Related Samples
- [DeclaredAgeRangeSample by dalexsoto](https://github.com/dalexsoto/DeclaredAgeRangeSample) - Swift wrapper and iOS binding reference

## License

This sample is provided as-is for educational purposes.

## Notes

### Mac Catalyst Support

**Mac Catalyst is NOT supported** for the following technical reasons:

1. **Framework Unavailability**: Apple's `DeclaredAgeRange` module does not exist in Mac Catalyst SDK
   - Verified in Xcode 16.0 and 16.1
   - Native Swift apps also fail with "No such module DeclaredAgeRange" when targeting Catalyst
   - Module is iOS-only, not available for macOS or Catalyst

2. **Build Errors**: Attempting to build for Mac Catalyst produces:
   ```
   error: module 'DeclaredAgeRange' not found
   ```

### Other Notes

- **Beta APIs**: Android Age Signals API is in beta; production use requires careful testing
- **Regional Limitations**: Age verification APIs have regional restrictions and requirements
- **iOS Version**: Requires iOS 26.0+.


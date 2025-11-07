---
name: .NET MAUI - Age Signals
description: Cross-platform age verification implementation for Android (Google Play Age Signals) and Windows (Age Consent API) using .NET MAUI with clean service architecture.
page_type: sample
languages:
  - csharp
  - xaml
products:
  - dotnet-maui
urlFragment: cross-platform-age-signals
---

# Age Signals (MAUI + Cross-Platform Age Verification)

A comprehensive .NET MAUI sample that demonstrates implementing age verification for Android and Windows applications. This sample shows how to integrate platform-specific age verification systems (Google Play Age Signals for Android and Windows Age Consent API for Windows) with a unified interface and clean architecture.

![Age Signals Demo](Images/age_signal_demo.png)

## What you'll learn

• How to implement cross-platform age verification in a .NET MAUI application
• How to create a unified age verification service interface for multiple platforms
• How to use Google Play Age Signals API (Android) for age verification
• How to use Windows Age Consent API (Windows.System.User) for age verification
• How to use dependency injection for platform-specific service implementations
• How to handle age verification checks with proper error handling
• Platform-specific best practices for Android and Windows age verification
• How to structure a service with proper initialization and error handling

## Prerequisites

• .NET 10.0 SDK or later
• Visual Studio 2022 17.13+ or Visual Studio Code with .NET MAUI extension
• Android SDK (for Android deployment)
• Windows SDK (for Windows deployment on Windows)
• Google Play Console account (for Android production age verification setup)
• Android device or emulator / Windows device for testing

## Features

### Core Age Verification Functionality

- **Age Signal Retrieval**: Query age verification status from Google Play / Windows
- **Platform Abstraction**: Unified interface across Android and Windows
- **Error Handling**: Graceful handling of age verification errors
- **Comprehensive Logging**: Debug logging for troubleshooting

### Architecture Components

- **IAgeSignalService**: Unified age verification service interface
- **Platforms/Android/AgeSignalService.cs**: Android implementation using Google Play Age Signals
- **Platforms/Windows/AgeSignalService.cs**: Windows implementation using Windows Age Consent API
- **Dependency Injection**: Platform-specific service registration
- **Clean UI**: Simple interface with DisplayAlert for error messaging

## Project Structure

```
AgeSignalSample/
├── Services/
│   └── IAgeSignalService.cs         # Unified age verification interface
├── Platforms/
│   ├── Android/
│   │   ├── AgeSignalService.cs      # Android age verification (Google Play Age Signals)
│   │   ├── AndroidManifest.xml      # Android permissions and configuration
│   │   └── MainActivity.cs          # Android main activity
│   └── Windows/
│       ├── AgeSignalService.cs      # Windows age verification (Age Consent API)
│       ├── Package.appxmanifest     # Windows package configuration
│       └── App.xaml.cs              # Windows app configuration
├── Models/
│   ├── AgeVerificationMethod.cs     # Verification method enumeration
│   ├── AgeVerificationRequest.cs    # Request model
│   └── AgeVerificationResult.cs     # Result model with age range
├── MainPage.xaml                    # Main page UI
├── MainPage.xaml.cs                 # Main page logic
└── MauiProgram.cs                   # Service registration
```

## How it's wired

• **`Services/IAgeSignalService.cs`**: Defines the unified contract for age verification operations including requesting age signals across all platforms.

• **`Platforms/Android/AgeSignalService.cs`**: Implements Android age verification using Google Play Age Signals API. Returns information about requirements since the API is not yet fully available.

• **`Platforms/Windows/AgeSignalService.cs`**: Implements Windows age verification using the Windows.System.User API with CheckUserAgeConsentGroupAsync method to determine age consent groups (Adult/Child).

• **`MauiProgram.cs`**: Registers the platform-specific age verification service implementation in the dependency injection container using conditional compilation.

• **`MainPage.xaml`**: Simple UI displaying platform information and a button to check age signals with result display.

• **`MainPage.xaml.cs`**: Handles the age verification button click, calls the service, and displays results using DisplayAlert for errors.

## Configuration

### Android Setup

1. **Google Play Console Configuration**:
   - Sign in to [Google Play Console](https://play.google.com/console)
   - Navigate to your app → Policy → App content
   - Configure age verification settings

2. **Requirements**:
   - App must be downloaded from Google Play Store (not sideloaded)
   - Must be in an applicable jurisdiction
   - Testing requires date after January 1, 2026 (when API becomes fully available)
   - User must have age verified in Google Play Store settings

3. **NuGet Package**:
   ```xml
   <PackageReference Include="Xamarin.Google.Android.Play.Age.Signals" Version="0.0.1-beta02" />
   ```

4. **API Key Points**:
   - Uses `AgeSignalsManagerFactory.Create(Activity)` to obtain the manager
   - Calls `CheckAgeSignals()` to retrieve age verification status
   - Returns age range information when available

### iOS Setup

*iOS implementation is planned for a future update using the Declared Age Range API.*

### Mac Catalyst Setup

*Mac Catalyst implementation is planned for a future update using the Declared Age Range API.*

### Windows Setup

1. **Package Manifest Configuration**:
   - Add the `userAccountInformation` capability to Package.appxmanifest:
   ```xml
   <Capabilities>
     <uap:Capability Name="userAccountInformation" />
   </Capabilities>
   ```

2. **Requirements**:
   - Windows 10.0.17134.0 or later
   - User must have a Microsoft account signed in
   - User must have age information configured in Microsoft account settings

3. **API Key Points**:
   - Uses `User.FindAllAsync()` to get the current user
   - Calls `CheckUserAgeConsentGroupAsync()` with `UserAgeConsentGroup.Adult` and `UserAgeConsentGroup.Child`
   - Returns age range based on `UserAgeConsentResult` (Included, NotIncluded, Unknown, NotEnforced, Ambiguous)
   - Handles `UnauthorizedAccessException` when capability is missing

4. **Target Framework**:
   ```xml
   <TargetFrameworks>net10.0-windows10.0.22000.0</TargetFrameworks>
   <SupportedOSPlatformVersion Condition="...== 'windows'">10.0.22000.0</SupportedOSPlatformVersion>
   ```

## Run the Application

### Android

1. Ensure Android SDK is properly configured
2. Set up an Android device or emulator (API 23+)
3. Build and deploy:

   ```bash
   dotnet build -f net10.0-android
   dotnet run -f net10.0-android
   ```

4. **Expected Behavior**:
   - Currently displays requirements message since API is not fully available until January 1, 2026
   - Will return actual age signals when requirements are met

### Windows

1. Ensure Windows SDK is properly configured (Windows only)
2. Set up a Windows device (Windows 11 recommended)
3. Build and deploy:

   ```bash
   dotnet build -f net10.0-windows10.0.22000.0
   dotnet run -f net10.0-windows10.0.22000.0
   ```

4. **Expected Behavior**:
   - Checks if user is 18+ using Adult consent group
   - If not adult, checks Child consent group for 0-17 age range
   - Returns age range or error messages based on consent status

### Key Features Demonstrated

**Platform Detection**: The app displays current platform information (Android/Windows).

**Age Verification Flow**: Tapping "Check Age Signals" initiates the platform-specific age verification flow with proper error handling.

**Visual Feedback**: Results are displayed with color-coded labels (Green for success, Red for errors).

**Error Handling**: Comprehensive error messages using DisplayAlert for user-friendly communication.

**Logging**: Debug logging throughout the verification process for troubleshooting.

## Dependencies

• **Microsoft.Maui.Controls**: Core MAUI framework
• **Xamarin.Google.Android.Play.Age.Signals**: Google Play Age Signals integration (Android only)
• **Microsoft.Extensions.Logging.Debug**: Debug logging support

## Architecture Patterns

### Service Pattern

- Unified age verification interface with platform-specific implementations
- Android: Google Play Age Signals API (AgeSignalsManagerFactory)
- Windows: Windows Age Consent API (Windows.System.User.CheckUserAgeConsentGroupAsync)
- Dependency injection for loose coupling

### Error Handling

- Comprehensive error handling in age verification operations
- User-friendly error messages via DisplayAlert
- Graceful degradation when age verification is unavailable
- Platform-specific exception handling (UnauthorizedAccessException on Windows)

### Logging

- Microsoft.Extensions.Logging.ILogger integration
- Debug output for verification flow tracking
- Error logging for troubleshooting

## Useful docs and resources

• Google Play Age Signals documentation — [developer.android.com/google/play/age](https://developer.android.com/google/play/age)
• Windows Age Consent API documentation — [learn.microsoft.com/uwp/api/windows.system.user.checkuserageconsentgroupasync](https://learn.microsoft.com/en-us/uwp/api/windows.system.user.checkuserageconsentgroupasync)
• .NET MAUI documentation — [learn.microsoft.com/dotnet/maui](https://learn.microsoft.com/dotnet/maui/)
• NuGet packages:
  ◦ [Microsoft.Maui.Controls](https://www.nuget.org/packages/Microsoft.Maui.Controls)
  ◦ [Xamarin.Google.Android.Play.Age.Signals](https://www.nuget.org/packages/Xamarin.Google.Android.Play.Age.Signals)

## Notes

• This sample demonstrates cross-platform age verification for Android (Google Play Age Signals) and Windows (Age Consent API)
• Android age verification requires:
  - App distributed via Google Play Store
  - Date after January 1, 2026 (when API becomes fully available)
  - User age verified in Google Play settings
  - Applicable jurisdiction
• Windows age verification requires:
  - `userAccountInformation` capability in Package.appxmanifest
  - Microsoft account signed in with age information
  - Windows 10.0.17134.0 or later
• iOS and Mac Catalyst implementations are planned for future updates
• Always test thoroughly on target platforms before publishing to production

## Security Considerations

• Validate age verification results server-side for production applications
• Handle edge cases like network interruptions during verification
• Secure storage of age verification information
• For Windows: Ensure `userAccountInformation` capability is properly declared
• For Android: Ensure app is properly configured in Google Play Console
• Never bypass age verification for production scenarios
• Respect user privacy and data protection regulations (GDPR, COPPA, etc.)

## Current Implementation Status

✅ **Android**: Service implemented with Google Play Age Signals API pattern (awaiting full API availability on January 1, 2026)

✅ **Windows**: Fully implemented with Windows Age Consent API (User.CheckUserAgeConsentGroupAsync)

⏳ **iOS**: Planned (will use Declared Age Range API)

⏳ **Mac Catalyst**: Planned (will use Declared Age Range API)

---
name: .NET MAUI - BillingService
description: Cross-platform billing implementation for Android (Google Play Billing) and iOS (StoreKit) using .NET MAUI with MVVM architecture.
page_type: sample
languages:
  - csharp
  - xaml
products:
  - dotnet-maui
urlFragment: cross-platform-billing-service
---

# BillingService (MAUI + Cross-Platform Billing)

A comprehensive .NET MAUI sample that demonstrates implementing in-app purchases for both Android and iOS applications. This sample shows how to integrate platform-specific billing systems (Google Play Billing for Android and StoreKit for iOS) with a unified interface and clean MVVM architecture.

![BillingService Demo](Images/billing_demo.png)

## What you'll learn

• How to implement cross-platform billing in a .NET MAUI application
• How to create a unified billing service interface for Android and iOS
• How to use Google Play Billing Client (Android) and StoreKit (iOS)
• How to use MVVM pattern with dependency injection for billing operations
• How to handle product listings, purchases, and purchase restoration on both platforms
• How to implement value converters for dynamic UI updates based on purchase state
• How to structure a billing service with proper initialization and error handling
• Platform-specific best practices for Android and iOS billing

## Prerequisites

• .NET 10.0 SDK or later
• Visual Studio 2022 17.13+ or Visual Studio Code with .NET MAUI extension
• Android SDK (for Android deployment)
• Xcode and iOS SDK (for iOS deployment on macOS)
• Google Play Console account (for Android production billing setup)
• Apple Developer account and App Store Connect access (for iOS production billing setup)
• Android device or emulator / iOS device or simulator for testing

## Features

### Core Billing Functionality

- **Product Discovery**: Retrieve available in-app products from Google Play / App Store
- **Purchase Flow**: Handle secure purchase transactions on both platforms
- **Purchase Restoration**: Restore previous purchases for users
- **Ownership Verification**: Check if products are already owned
- **Cross-Platform Abstraction**: Unified interface across Android and iOS

### Architecture Components

- **IBillingService**: Unified billing service interface
- **BaseBillingService**: Shared base functionality and business logic
- **Platforms/Android/BillingService.cs**: Android implementation using Google Billing Client v7
- **Platforms/iOS/BillingService.cs**: iOS implementation using Apple StoreKit 1
- **MVVM Pattern**: Clean separation with ViewModels and data binding
- **Dependency Injection**: Platform-specific service registration

### UI Features

- **Product Grid**: Display available products with pricing
- **Purchase Status**: Visual indicators for owned/unowned products
- **Loading States**: User feedback during billing operations
- **Error Handling**: Graceful handling of billing errors

## Project Structure

```
BillingService/
├── Services/
│   ├── IBillingService.cs           # Unified billing service interface
│   └── BaseBillingService.cs        # Shared base implementation
├── Platforms/
│   ├── Android/
│   │   ├── BillingService.cs        # Android billing (Google Play Billing v7)
│   │   ├── AndroidManifest.xml      # Android permissions and configuration
│   │   └── MainActivity.cs          # Android main activity
│   └── iOS/
│       ├── BillingService.cs        # iOS billing (StoreKit 1)
│       ├── Info.plist               # iOS configuration
│       └── AppDelegate.cs           # iOS app delegate
├── Models/
│   ├── Product.cs                   # Product data model
│   └── PurchaseResult.cs           # Purchase result model
├── ViewModels/
│   ├── BaseViewModel.cs            # Base ViewModel with INotifyPropertyChanged
│   └── ProductsViewModel.cs        # Products page ViewModel
├── Views/
│   ├── ProductsPage.xaml           # Products listing page
│   └── ProductsPage.xaml.cs        # Code-behind
└── Converters/
    └── ValueConverters.cs          # XAML value converters
```

## How it's wired

• **`Services/IBillingService.cs`**: Defines the unified contract for billing operations including initialization, product retrieval, and purchase handling across platforms.

• **`Services/BaseBillingService.cs`**: Provides shared business logic, product definitions, and ownership tracking used by both Android and iOS.

• **`Platforms/Android/BillingService.cs`**: Implements Android billing using Google Play Billing Client v7 with support for product queries, purchases, and restoration.

• **`Platforms/iOS/BillingService.cs`**: Implements iOS billing using StoreKit 1 APIs with transaction observers and purchase restoration.

• **`MauiProgram.cs`**: Registers the billing service implementation (`Services.BillingService`) and ViewModels in the dependency injection container.

• **`ViewModels/ProductsViewModel.cs`**: Exposes billing operations as commands, manages product collections, and handles UI state updates (platform-agnostic).

• **`Views/ProductsPage.xaml`**: CollectionView displaying products with purchase buttons and visual indicators for ownership status.

• **`Converters/ValueConverters.cs`**: Provides XAML converters for boolean-to-text, boolean-to-color, and inverse boolean transformations.

## Configuration

### Android Setup

1. **Product Configuration**:
   Update the product IDs in your billing service to match those configured in Google Play Console:
   - Sign in to [Google Play Console](https://play.google.com/console)
   - Navigate to your app → Monetize → In-app products
   - Create products with IDs: `Team_license`, `Global_license`, `Unlimited_license`

2. **Testing**:
   - Add license testers in Google Play Console
   - Use internal testing track for testing purchases

### iOS Setup

1. **Product Configuration**:
   Update the product IDs in your billing service to match those configured in App Store Connect:
   - Sign in to [App Store Connect](https://appstoreconnect.apple.com/)
   - Navigate to your app → Features → In-App Purchases
   - Create products with IDs: `Team_license`, `Global_license`, `Unlimited_license`

2. **Testing**:
   - Create sandbox tester accounts in App Store Connect
   - Use sandbox account on device for testing purchases

3. **Additional Requirements**:
   - Sign Paid Applications Agreement in App Store Connect
   - Configure tax and banking information

## Run the Application

### Android

1. Ensure Android SDK is properly configured
2. Set up an Android device or emulator
3. Build and deploy:

   ```bash
   dotnet build -f net10.0-android
   dotnet run -f net10.0-android
   ```

### iOS

1. Ensure Xcode and iOS SDK are installed (macOS only)
2. Set up an iOS device or simulator
3. Build and deploy:

   ```bash
   dotnet build -f net10.0-ios
   dotnet run -f net10.0-ios
   ```

### Key Features Demonstrated

**Product Listing**: The app retrieves and displays available in-app products with their details and pricing.

**Purchase Flow**: Tapping a product initiates the platform-specific purchase flow (Google Play on Android, StoreKit on iOS) with proper error handling.

**Visual Feedback**: Products show different states (owned/not owned) with color coding and text changes.

**Restoration**: Users can restore previous purchases across devices.

## Dependencies

• **Microsoft.Maui.Controls**: Core MAUI framework
• **Xamarin.Android.Google.BillingClient**: Google Play Billing integration
• **Syncfusion.Maui.Toolkit**: Additional UI components
• **Microsoft.Extensions.Logging.Debug**: Debug logging support

## Architecture Patterns

### MVVM Implementation

- Uses `INotifyPropertyChanged` for data binding
- Commands for user interactions
- Separation of concerns between Views and ViewModels

### Service Pattern

- Unified billing interface with platform-specific implementations
- Android: Google Play Billing Client v7
- iOS: StoreKit 1 with transaction observers
- Dependency injection for loose coupling

### Error Handling

- Comprehensive error handling in billing operations
- User-friendly error messages
- Graceful degradation when billing is unavailable

## Useful docs and resources

• Google Play Billing documentation — [developer.android.com/google/play/billing](https://developer.android.com/google/play/billing)
• Apple StoreKit documentation — [developer.apple.com/storekit](https://developer.apple.com/storekit/)
• .NET MAUI documentation — [learn.microsoft.com/dotnet/maui](https://learn.microsoft.com/dotnet/maui/)
• NuGet packages:
  ◦ [Microsoft.Maui.Controls](https://www.nuget.org/packages/Microsoft.Maui.Controls)
  ◦ [Xamarin.Android.Google.BillingClient](https://www.nuget.org/packages/Xamarin.Android.Google.BillingClient)
  ◦ [Syncfusion.Maui.Toolkit](https://www.nuget.org/packages/Syncfusion.Maui.Toolkit)

## Notes

• This sample demonstrates cross-platform billing for both Android (Google Play Billing) and iOS (StoreKit)
• Testing in-app purchases requires:
  - Android: Google Play Console setup and signed APKs
  - iOS: App Store Connect setup and sandbox tester accounts
• Always test thoroughly on both platforms before publishing to production

## Security Considerations

• Validate purchases server-side for production applications
• Implement receipt verification
• Handle edge cases like network interruptions during purchases
• Secure storage of purchase information

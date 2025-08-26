---
name: .NET MAUI - BillingService
description: Comprehensive Android billing implementation using Google Play Billing with .NET MAUI and MVVM architecture.
page_type: sample
languages:
  - csharp
  - xaml
products:
  - dotnet-maui
urlFragment: android-billing-service
---

# BillingService (MAUI + Google Play Billing)

A comprehensive .NET MAUI sample that demonstrates implementing Google Play Billing for Android applications. This sample shows how to integrate in-app purchases, product listings, and purchase management using the Google Billing Client library with a clean MVVM architecture.

![BillingService Demo](Images/billing_demo.png)

## What you'll learn

• How to implement Google Play Billing in a .NET MAUI Android application
• How to create a billing service for Android using Google Billing Client
• How to use MVVM pattern with CommunityToolkit.Mvvm for billing operations
• How to handle product listings, purchases, and purchase restoration
• How to implement value converters for dynamic UI updates based on purchase state
• How to structure a billing service with proper initialization and error handling

## Prerequisites

• .NET 10.0 SDK or later
• Visual Studio 2022 17.13+ or Visual Studio Code with .NET MAUI extension
• Android SDK (for Android deployment)
• Google Play Console account (for production billing setup)
• Android device or emulator for testing

## Features

### Core Billing Functionality

- **Product Discovery**: Retrieve available in-app products from Google Play
- **Purchase Flow**: Handle secure purchase transactions
- **Purchase Restoration**: Restore previous purchases for users
- **Ownership Verification**: Check if products are already owned

### Architecture Components

- **IBillingService**: Billing service interface
- **AndroidBillingService**: Android implementation using Google Billing Client
- **BaseBillingService**: Shared base functionality
- **MVVM Pattern**: Clean separation with ViewModels and data binding

### UI Features

- **Product Grid**: Display available products with pricing
- **Purchase Status**: Visual indicators for owned/unowned products
- **Loading States**: User feedback during billing operations
- **Error Handling**: Graceful handling of billing errors

## Project Structure

```
BillingService/
├── Services/
│   ├── IBillingService.cs           # Billing service interface
│   ├── BaseBillingService.cs        # Base implementation
│   └── AndroidBillingService.cs     # Android billing implementation
├── Models/
│   ├── Product.cs                   # Product data model
│   └── PurchaseResult.cs           # Purchase result model
├── ViewModels/
│   ├── BaseViewModel.cs            # Base ViewModel with INotifyPropertyChanged
│   └── ProductsViewModel.cs        # Products page ViewModel
├── Views/
│   ├── ProductsPage.xaml           # Products listing page
│   └── ProductsPage.xaml.cs        # Code-behind
├── Converters/
│   └── ValueConverters.cs          # XAML value converters
└── Platforms/
    └── Android/
        ├── AndroidManifest.xml     # Android permissions and configuration
        └── MainActivity.cs         # Android main activity
```

## How it's wired

• **`Services/IBillingService.cs`**: Defines the contract for billing operations including initialization, product retrieval, and purchase handling.

• **`Services/AndroidBillingService.cs`**: Implements the Android billing logic using `Xamarin.Android.Google.BillingClient` package.

• **`MauiProgram.cs`**: Registers the billing service and ViewModels in the dependency injection container.

• **`ViewModels/ProductsViewModel.cs`**: Exposes billing operations as commands, manages product collections, and handles UI state updates.

• **`Views/ProductsPage.xaml`**: CollectionView displaying products with purchase buttons and visual indicators for ownership status.

• **`Converters/ValueConverters.cs`**: Provides XAML converters for boolean-to-text, boolean-to-color, and inverse boolean transformations.

## Configuration

### Android Setup

1. **Product Configuration**:
   Update the product IDs in your billing service to match those configured in Google Play Console.

## Run the Application

### Android

1. Ensure Android SDK is properly configured
2. Set up an Android device or emulator
3. Build and deploy:

### Key Features Demonstrated

**Product Listing**: The app retrieves and displays available in-app products with their details and pricing.

**Purchase Flow**: Tapping a product initiates the Google Play purchase flow with proper error handling.

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

- Billing operations through interfaces
- Android implementation using Google Billing Client
- Dependency injection for loose coupling

### Error Handling

- Comprehensive error handling in billing operations
- User-friendly error messages
- Graceful degradation when billing is unavailable

## Useful docs and resources

• Google Play Billing documentation — [developer.android.com/google/play/billing](https://developer.android.com/google/play/billing)
• .NET MAUI documentation — [learn.microsoft.com/dotnet/maui](https://learn.microsoft.com/dotnet/maui/)
• NuGet packages:
◦ [Microsoft.Maui.Controls](https://www.nuget.org/packages/Microsoft.Maui.Controls)
◦ [Xamarin.Android.Google.BillingClient](https://www.nuget.org/packages/Xamarin.Android.Google.BillingClient)
◦ [Syncfusion.Maui.Toolkit](https://www.nuget.org/packages/Syncfusion.Maui.Toolkit)

## Notes

• This sample focuses on Android implementation using Google Play Billing
• Testing in-app purchases requires proper Google Play Console setup and signed APKs
• Always test thoroughly before publishing to production

## Security Considerations

• Validate purchases server-side for production applications
• Implement receipt verification
• Handle edge cases like network interruptions during purchases
• Secure storage of purchase information

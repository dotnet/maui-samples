# .NET 6.0.0 Mobile Samples

_This is an *early* preview of Mobile (iOS/Android) in .NET 6 **not for production use**. Expect breaking changes as this is still in development for .NET 6._

If you are looking for the absolute newest download links see the
[develop](https://github.com/dotnet/net6-mobile-samples/tree/develop)
branch.

## Installing with .NET MAUI Check Tool

This is a community supported, open source, global dotnet tool intended to help evaluation your development environment and help you install / configure everything you need to build a .NET MAUI application.

Install: `dotnet tool install -g redth.net.maui.check`
Run: `maui-check --dev`

This will evaluate your environment and in most cases optionally install / configure missing components for you, such as:
 - OpenJdk / AndroidSDK
 - .NET 6 Preview SDK
 - .NET MAUI / iOS / Android workloads and packs
 - .NET MAUI Templates
 - Workload Resolver .sentinel files for dotnet and Visual Studio Windows/Mac

For more information and source code, visit [redth/dotnet-maui-check](https://github.com/redth/dotnet-maui-check)


## Installing with Official Preview Installers

If you prefer to install everything manually, you can find all of the official installer links below:

* Windows: [dotnet-sdk-6.0.100-preview.3.21202.5-win-x64.exe](https://dotnetcli.azureedge.net/dotnet/Sdk/6.0.100-preview.3.21202.5/dotnet-sdk-6.0.100-preview.3.21202.5-win-x64.exe)
* macOS: [dotnet-sdk-6.0.100-preview.3.21202.5-osx-x64.pkg](https://dotnetcli.azureedge.net/dotnet/Sdk/6.0.100-preview.3.21202.5/dotnet-sdk-6.0.100-preview.3.21202.5-osx-x64.pkg)

You will also need to install builds of the iOS and Android workloads:

Android:

* Windows: [Microsoft.NET.Workload.Android.11.0.200-preview.3.196.msi](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4624420/6.0.1xx-preview3/7d6cd1cde4182d7db2cfc5d0b55364c972b6d34f/Microsoft.NET.Workload.Android.11.0.200.196.msi)
* macOS: [Microsoft.NET.Workload.Android-11.0.200-preview.3.196.pkg](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4624420/6.0.1xx-preview3/7d6cd1cde4182d7db2cfc5d0b55364c972b6d34f/Microsoft.NET.Workload.Android-11.0.200-preview.3.196.pkg)

iOS:

* Windows: [Microsoft.NET.Workload.iOS.14.4.100-preview.3.1326.msi](https://bosstoragemirror.azureedge.net/wrench/6.0.1xx-preview3/f68d4d9c2a342daf9eaad364ccbe252e009d3901/4623693/package/Microsoft.NET.Workload.iOS.14.4.100-preview.3.1326.msi)
* macOS: [Microsoft.iOS.Bundle.14.4.100-preview.3.1326.pkg](https://bosstoragemirror.azureedge.net/wrench/6.0.1xx-preview3/f68d4d9c2a342daf9eaad364ccbe252e009d3901/4623693/package/notarized/Microsoft.iOS.Bundle.14.4.100-preview.3.1326.pkg)

Mac (Cocoa/AppKit):

* macOS: [Microsoft.macOS.Bundle.11.1.100-preview.3.1379.pkg](https://bosstoragemirror.azureedge.net/wrench/6.0.1xx-preview3/f68d4d9c2a342daf9eaad364ccbe252e009d3901/4623693/package/notarized/Microsoft.macOS.Bundle.11.1.100-preview.3.1379.pkg)

Mac Catalyst (UIKit):

* macOS: [Microsoft.MacCatalyst.Bundle.14.3.100-preview.3.471.pkg](https://bosstoragemirror.azureedge.net/wrench/6.0.1xx-preview3/f68d4d9c2a342daf9eaad364ccbe252e009d3901/4623693/package/notarized/Microsoft.MacCatalyst.Bundle.14.3.100-preview.3.471.pkg)

_NOTE: newer builds of .NET *may* work, but your mileage may vary.
The workload installers enable a feature flag file via
`sdk/6.0.100-*/EnableWorkloadResolver.sentinel`, which would
need to be created manually for other .NET 6 versions. You can find
the full list of builds at the [dotnet/installer][dotnet/installer]
repo._

Projects:

* HelloMaui - a multi-targeted .NET MAUI Single Project for iOS and Android
* HelloAndroid - a native Android application
* HelloiOS - a native iOS application
* HelloMacCatalyst - a native Mac Catalyst application

[dotnet/installer]: https://github.com/dotnet/installer#installers-and-binaries
[net6preview1]: https://github.com/dotnet/net6-mobile-samples/releases/tag/6.0.1xx-preview1

## Android

Prerequisites:

* You will need the Android SDK installed as well as `Android SDK Platform 30`. Simplest way to get this is to install the current Xamarin workload and go to `Tools > Android > Android SDK Manager` from within Visual Studio.

For example, to build the Android project:

    dotnet build HelloAndroid

You can launch the Android project to an attached emulator or device via:

    dotnet build HelloAndroid -t:Run

## iOS

Prerequisites:

* Xcode 12.4. Earlier versions won't work.

To build the iOS project:

    dotnet build HelloiOS

To launch the iOS project on a simulator:

    dotnet build HelloiOS -t:Run

## .NET MAUI

To launch the .NET MAUI project, you will need to specify a `$(TargetFramework)` via the `-f` switch:

    dotnet build HelloMaui -t:Run -f net6.0-android
    dotnet build HelloMaui -t:Run -f net6.0-ios
    dotnet build HelloMaui -t:Run -f net6.0-maccatalyst

## Using IDEs

IDE integration into Visual Studio, Visual Studio for Mac, and Visual Studio Code are a work in progress. 

### Visual Studio

Currently, you can use Visual Studio 2019 16.9 on Windows (with the Xamarin workload) with a few manual steps to run iOS & Android apps built on .NET 6.

Open an Administrator command prompt to enable the `EnableWorkloadResolver.sentinel` feature flag:

    > cd "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\SdkResolvers\Microsoft.DotNet.MSBuildSdkResolver"
    > type NUL > EnableWorkloadResolver.sentinel

Or in an Administrator `powershell` prompt:

    > cd "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\SdkResolvers\Microsoft.DotNet.MSBuildSdkResolver"
    > '' > EnableWorkloadResolver.sentinel

> NOTE: your path to Visual Studio may vary, depending on where you selected to install it. 
> `Enterprise`, `Professional`, or `Community` might be correct depending on the SKU you have installed.

This command creates an empty file that enables .NET workload support.

Restart Visual Studio after making this change.

### Visual Studio for Mac

Visual Studio for Mac is not supported at this time, but will be coming in a future release.

### .NET MAUI

.NET MAUI bassed projects can be open in Visual Studio and Visual Studio for Mac, however can not be run or debugged directly from the IDEs at this time.

### iOS from Visual Studio

To build and debug .NET 6 iOS applications from Visual Studio 2019 you must manually intall the .NET 6 SDK and iOS workloads on both **Windows and macOS** (Mac build host).

If while connecting Visual Studio to your Mac through XMA you are prompted to install a different version of the SDK, you can ignore that since it refers to the legacy one.

> Note: currently only the iOS simulator is supported (not the remoted simulator).

### Mac Catalyst from Visual Studio for Mac

Running and debugging apps from Visual Studio for Mac is not supported at this time..

### Known Issues - Visual Studio & Visual Studio for Mac

* There are no project property pages available for both iOS and Android
* Editors (i.e. Manifest editor, Entitlements editor, etc.) will fail to open, so as a workaround please open those files with the XML editor.

### Visual Studio Code

Support has been added to allow debugging of **Android** based apps in Visual Studio Code. Open the `net6-mobile-samples.code-workspace` in Visual Studio Code.

    > code net6-mobile-samples.code-workspace

To build your application use open the Command Pallette and select `Run Build Task`. Select `Build` and then the `Target` you want to run. Available targets are:

* `Build` : Builds the Project.
* `Install` : Installs the Application on a Device or Emulator.
* `Clean` : Clean the Project.

You can then select the `Project` and then the `Configuration` (`Debug` or `Release`) you want to `Build`.

To Debug goto the `Run` Tab and make sure `Debug` is selected. Click the Run button. You will be prompted on which project you wish to run, then asked which `TargetFramework` you want to target. For now only `net6.0-android` is supported. You will then be asked if you want to attach the debugger. Finally you will be asked which configuration you wish to use `Debug` or `Release`. After this the application should deploy and run, breakpoints should behave as normal.

Note: You will need to build your application at least once via `Run Build Task`. This is to that NuGet packages are restored correctly. This should not be required once [dotnet#15485](https://github.com/dotnet/sdk/issues/15485) is resolved.

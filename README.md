# .NET 6.0.0 Mobile Samples

_This is an *early* preview of Mobile (iOS/Android) in .NET 6 **not for production use**. Expect breaking changes as this is still in development for .NET 6._

If you are looking for the absolute newest download links see the
[develop](https://github.com/dotnet/net6-mobile-samples/tree/develop)
branch.

This repo requires a specific build of .NET 6:

* Windows: [dotnet-sdk-6.0.100-preview.2.21155.3-win-x64.exe](https://download.visualstudio.microsoft.com/download/pr/2290b039-85d8-4d95-85f7-edbd9fcd118d/a64bef89625bc61db2a6832878610214/dotnet-sdk-6.0.100-preview.2.21155.3-win-x64.exe)
* macOS: [dotnet-sdk-6.0.100-preview.2.21155.3-osx-x64.pkg](https://download.visualstudio.microsoft.com/download/pr/5e10dc75-294e-49f4-972e-218ae86191a3/e46d3533c30c8a864252a334820263a9/dotnet-sdk-6.0.100-preview.2.21155.3-osx-x64.pkg)

You will also need to install builds of the iOS and Android workloads:

Android:

* Windows: [Microsoft.NET.Workload.Android.11.0.200.148.msi](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4534967/main/f4d8fe238b15eadfc7842749bf13e5fca3e2f2d2/Microsoft.NET.Workload.Android.11.0.200.148.msi)
* macOS: [Microsoft.NET.Workload.Android-11.0.200.148.pkg](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4534967/main/f4d8fe238b15eadfc7842749bf13e5fca3e2f2d2/Microsoft.NET.Workload.Android-11.0.200-ci.f4d8fe238b15eadfc7842749bf13e5fca3e2f2d2.148.pkg)

iOS:

* Windows: [Microsoft.NET.Workload.iOS.14.4.100-ci.main.1271.msi](https://bosstoragemirror.azureedge.net/wrench/main/c8b6bc6c85a0067387ee298ef5e7d55992be5f0a/4590608/package/Microsoft.NET.Workload.iOS.14.4.100-ci.main.1271.msi)
* macOS: [Microsoft.iOS.Bundle.14.4.100-ci.main.1271.pkg](https://bosstoragemirror.azureedge.net/wrench/main/c8b6bc6c85a0067387ee298ef5e7d55992be5f0a/4590608/package/Microsoft.iOS.Bundle.14.4.100-ci.main.1271.pkg)

Mac (Cocoa/AppKit):

* macOS: [Microsoft.macOS.Bundle.11.1.100-ci.main.1324.pkg](https://bosstoragemirror.azureedge.net/wrench/main/c8b6bc6c85a0067387ee298ef5e7d55992be5f0a/4590608/package/Microsoft.macOS.Bundle.11.1.100-ci.main.1324.pkg)

Mac Catalyst (UIKit):

* macOS: [Microsoft.MacCatalyst.Bundle.14.3.100-ci.main.416.pkg](https://bosstoragemirror.azureedge.net/wrench/main/c8b6bc6c85a0067387ee298ef5e7d55992be5f0a/4590608/package/Microsoft.MacCatalyst.Bundle.14.3.100-ci.main.416.pkg)

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

> NOTE: You may need to add the `--no-restore` switch until
> [dotnet#15485](https://github.com/dotnet/sdk/issues/15485) is
> resolved.

    dotnet build HelloMaui -t:Run -f net6.0-android
    dotnet build HelloMaui -t:Run -f net6.0-ios
    dotnet build HelloMaui -t:Run -f net6.0-maccatalyst

## Using IDEs

Currently, you can use Visual Studio 2019 16.9 on Windows (with the
Xamarin workload) with a few manual steps.

Open an Administrator command prompt to enable the
`EnableWorkloadResolver.sentinel` feature flag:

    > cd "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\SdkResolvers\Microsoft.DotNet.MSBuildSdkResolver"
    > echo > EnableWorkloadResolver.sentinel

Or in an Administrator `powershell` prompt:

    > cd "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\SdkResolvers\Microsoft.DotNet.MSBuildSdkResolver"
    > '' > EnableWorkloadResolver.sentinel

> NOTE: your path to Visual Studio may vary, depending on where you
> selected to install it. `Enterprise`, `Professional`, or `Community`
> might be correct depending on the SKU you have installed.

This command creates an empty file that enables .NET workload support.
Restart Visual Studio after making this change.

Visual Studio for Mac support will be coming in a future release.

### iOS from Visual Studio

To build and debug .NET 6 iOS applications from Visual Studio 2019 you
must manually intall the .NET 6 SDK and iOS workloads on both
**Windows and macOS** (Mac build host).

If while connecting Visual Studio to your Mac through XMA you are
prompted to install a different version of the SDK, you can ignore
that since it refers to the legacy one.

> Note: currently only the iOS simulator is supported.

### Mac Catalyst from Visual Studio for Mac

Running and debugging apps from Visual Studio for Mac does not work yet.

### Known Issues

* There are no project property pages available for both iOS and
  Android
* Editors (i.e. Manifest editor, Entitlements editor, etc.) will fail
  to open, so as a workaround please open those files with the XML
  editor.

### Android from VSCode

Support has been added to allow debugging of Android based apps in
VSCode. Open the `net6-mobile-samples.code-workspace` in VSCode.

    > code net6-mobile-samples.code-workspace

To build your application use open the Command Pallette and select
`Run Build Task`. Select `Build` and then the `Target` you want to
run. Available targets are:

* `Build` : Builds the Project.
* `Install` : Installs the Application on a Device or Emulator.
* `Clean` : Clean the Project.

You can then select the `Project` and then the `Configuration`
(`Debug` or `Release`) you want to `Build`.

To Debug goto the `Run` Tab and make sure `Debug` is selected. Click
the Run button. You will be prompted on which project you wish to run,
then asked which `TargetFramework` you want to target. For now only
`net6.0-android` is supported. You will then be asked if you want to
attach the debugger. Finally you will be asked which configuration you
wish to use `Debug` or `Release`. After this the application should
deploy and run, breakpoints should behave as normal.

Note: You will need to build your application at least once via
`Run Build Task`. This is to that NuGet packages are restored correctly.
This should not be required once [dotnet#15485](https://github.com/dotnet/sdk/issues/15485) is resolved.

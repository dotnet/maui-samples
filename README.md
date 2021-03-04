# .NET 6.0.0 Mobile Samples

_This is an *early* preview of Mobile (iOS/Android) in .NET 6 **not for production use**. Expect breaking changes as this is still in development for .NET 6._

If you are looking for the absolute newest download links see the
[develop](https://github.com/dotnet/net6-mobile-samples/tree/develop)
branch.

This repo requires a specific build of .NET 6:

* Windows: [dotnet-sdk-6.0.100-preview.2.21114.3-win-x64.exe](https://dotnetcli.blob.core.windows.net/dotnet/Sdk/6.0.100-preview.2.21114.3/dotnet-sdk-6.0.100-preview.2.21114.3-win-x64.exe)
* macOS: [dotnet-sdk-6.0.100-preview.2.21114.3-osx-x64.pkg](https://dotnetcli.blob.core.windows.net/dotnet/Sdk/6.0.100-preview.2.21114.3/dotnet-sdk-6.0.100-preview.2.21114.3-osx-x64.pkg)

You will also need to install builds of the iOS and Android workloads:

Android:

* Windows: [Microsoft.NET.Workload.Android.11.0.200.136.msi](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4525053/master/8cd0b47032d267a63b449a63c0839c60cbd976f1/Microsoft.NET.Workload.Android.11.0.200.136.msi)
* macOS: [Microsoft.NET.Workload.Android-11.0.200.136.pkg](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4525053/master/8cd0b47032d267a63b449a63c0839c60cbd976f1/Microsoft.NET.Workload.Android-11.0.200-ci.8cd0b47032d267a63b449a63c0839c60cbd976f1.136.pkg)

iOS:

These builds are newer than [.NET 6 Preview 1][net6preview1] and
require Xcode 12.4. Use downloads from [.NET 6 Preview
1][net6preview1] for Xcode 12.3:

* Windows: [Microsoft.NET.Workload.iOS.14.4.100-ci.main.1167.msi](https://bosstoragemirror.azureedge.net/wrench/main/80ed9d81bcf66afa2d800bc362e467180dbf246c/4524716/package/Microsoft.NET.Workload.iOS.14.4.100-ci.main.1167.msi)
* macOS: [Microsoft.iOS.Bundle.14.4.100-ci.main.1167.pkg](https://bosstoragemirror.azureedge.net/wrench/main/80ed9d81bcf66afa2d800bc362e467180dbf246c/4524716/package/notarized/Microsoft.iOS.Bundle.14.4.100-ci.main.1167.pkg)

_NOTE: newer builds of .NET *may* work, but your mileage may vary.
The workload installers enable a feature flag file via
`sdk/6.0.100-preview.1.21103.13/EnableWorkloadResolver.sentinel`, which would
need to be created manually for other .NET 6 versions. You can find
the full list of builds at the [dotnet/installer][dotnet/installer]
repo._

Projects:

* HelloAndroid - a native Android application
* HelloiOS - a native iOS application
* HelloForms - a multi-targeted Xamarin.Forms application for iOS and Android (will migrate to MAUI in a later preview)

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

## Xamarin.Forms / MAUI

*In later previews, this sample will be migrated to use MAUI*

To launch the Forms project, you will need to specify a `$(TargetFramework)` via the `-f` switch:

    dotnet build HelloForms -t:Run -f net6.0-android
    dotnet build HelloForms -t:Run -f net6.0-ios

> NOTE: You may need to add the `--no-restore` switch until
> [dotnet#15485](https://github.com/dotnet/sdk/issues/15485) is
> resolved.

## Using IDEs

Currently, you can use Visual Studio 2019 16.9 Preview 4 on Windows
(with the Xamarin workload) with a few manual steps.

Open an Administrator command prompt to enable the
`EnableWorkloadResolver.sentinel` feature flag:

    > cd "C:\Program Files (x86)\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin\SdkResolvers\Microsoft.DotNet.MSBuildSdkResolver"
    > echo > EnableWorkloadResolver.sentinel

> NOTE: your path to Visual Studio may vary, depending on where you
> selected to install it. `Preview` is the default folder for Visual
> Studio Preview versions.

Restart Visual Studio after making this change.

### iOS from Visual Studio

To build and debug .NET 6 iOS applications from Visual Studio 2019 you
must manually intall the .NET 6 SDK and iOS workloads on both
**Windows and macOS** (Mac build host).

If while connecting Visual Studio to your Mac through XMA you are
prompted to install a different version of the SDK, you can ignore
that since it refers to the legacy one.

> Note: currently only the iOS simulator is supported.

### Known Issues

* There are no project property pages available for both iOS and
  Android
* Editors (i.e. Manifest editor, Entitlements editor, etc.) will fail
  to open, so as a workaround please open those files with the XML
  editor.

## Workarounds

These are notes for things we had to workaround for these samples to work.

### NuGet

Currently, NuGet is not able to restore existing Xamarin.Android/iOS
packages for a .NET 6 project. We tried `$(AssetTargetFallback)`,
however, this option does not work in combination with transitive
dependencies. The `Xamarin.AndroidX.*` set of NuGet packages has a
complex dependency tree.

Additionally, we had some problems with the Xamarin.Forms NuGet
package listing the same assembly in both:

* `lib\netstandard2.0\Xamarin.Forms.Platform.dll`
* `lib\MonoAndroid10.0\Xamarin.Forms.Platform.dll`

For now we added workarounds in `xamarin-android`, see
[xamarin-android#4663](https://github.com/xamarin/xamarin-android/pull/4663).

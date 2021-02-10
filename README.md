# net6-samples

_This is an *early* preview of Xamarin in .NET 6 **not for production use**. Expect breaking changes as Xamarin is still in development for .NET 6._

This repo requires a specific build of .NET 6:

* Windows: [dotnet-sdk-6.0.100-preview.1.21103.13-win-x64.exe](https://dotnetcli.azureedge.net/dotnet/Sdk/6.0.100-preview.1.21103.13/dotnet-sdk-6.0.100-preview.1.21103.13-win-x64.exe)
* macOS: [dotnet-sdk-6.0.100-preview.1.21103.13-osx-x64.pkg](https://dotnetcli.azureedge.net/dotnet/Sdk/6.0.100-preview.1.21103.13/dotnet-sdk-6.0.100-preview.1.21103.13-osx-x64.pkg)

You will also need to install builds of the iOS and Android workloads:

Android:

* Windows: [Microsoft.NET.Workload.Android.11.0.200.85.msi](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4451481/master/05bb8e0eae11ae6a73838b13cf91ee2433169dff/Microsoft.NET.Workload.Android.11.0.200.85.msi)
* macOS: [Microsoft.NET.Workload.Android-11.0.200-ci.master.85.pkg](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/net6/4451481/master/05bb8e0eae11ae6a73838b13cf91ee2433169dff/Microsoft.NET.Workload.Android-11.0.200-ci.master.85.pkg)

iOS:

* Windows: [Microsoft.NET.Workload.iOS.14.3.100-ci.main.1079.msi](https://bosstoragemirror.azureedge.net/wrench/main/f01fde5cd9a7ffffcdc8d241200c35988700fa00/4449408/package/Microsoft.NET.Workload.iOS.14.3.100-ci.main.1079.msi)
* macOS: [Microsoft.iOS.Bundle.14.3.100-ci.main.1079.pkg](https://bosstoragemirror.azureedge.net/wrench/main/f01fde5cd9a7ffffcdc8d241200c35988700fa00/4449408/package/notarized/Microsoft.iOS.Bundle.14.3.100-ci.main.1079.pkg)

_NOTE: newer builds of .NET *may* work, but your mileage may vary.
The workload installers enable a feature flag file via
`sdk/6.0.100-preview.1.21103.13/EnableWorkloadResolver.sentinel`, which would
need to be created manually for other .NET 6 versions. You can find
the full list of builds at the [dotnet/installer][dotnet/installer]
repo._

Projects:

* HelloAndroid - a native Xamarin.Android application
* HelloiOS - a native Xamarin.iOS application
* HelloForms - a multi-targeted Xamarin.Forms application for iOS and Android.

[dotnet/installer]: https://github.com/dotnet/installer#installers-and-binaries

## Android

Prerequisites:

* You will need the Android SDK installed as well as `Android SDK Platform 30`. Simplest way to get this is to install the current Xamarin workload and go to `Tools > Android > Android SDK Manager` from within Visual Studio.

For example, to build the Android project:

    dotnet build HelloAndroid

You can launch the Android project to an attached emulator or device via:

    dotnet build HelloAndroid -t:Run

## iOS

Prerequisites:

* Xcode 12.3. Earlier versions won't work.

To build the iOS project:

    dotnet build HelloiOS

To launch the iOS project on a simulator:

    dotnet build HelloiOS -t:Run

## Xamarin.Forms

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

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit [https://cla.opensource.microsoft.com](https://cla.opensource.microsoft.com).

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

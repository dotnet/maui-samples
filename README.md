# net6-samples

_This is an *early* preview of Xamarin in .NET 6 **not for production use**. Expect breaking changes as Xamarin is still in development for .NET 6._

This repo requires a specific build of .NET 5 rtm:

* Windows: [dotnet-sdk-5.0.100-rtm.20509.5-win-x64.exe](https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.100-rtm.20509.5/dotnet-sdk-5.0.100-rtm.20509.5-win-x64.exe)
* macOS: [dotnet-sdk-5.0.100-rtm.20509.5-osx-x64.pkg](https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.100-rtm.20509.5/dotnet-sdk-5.0.100-rtm.20509.5-osx-x64.pkg)

You will also need to install builds of the iOS and Android workloads:

Android:
* Windows: [Microsoft.NET.Workload.Android.11.0.100.209.msi](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/4183754/master/57c5a5fde5efd23f5958cfd8119b7f9c31d9e39d/Microsoft.NET.Workload.Android.11.0.100.209.msi)
* macOS: [Microsoft.NET.Workload.Android-11.0.100-ci.master.209.pkg](https://dl.internalx.com/vsts-devdiv/Xamarin.Android/public/4183754/master/57c5a5fde5efd23f5958cfd8119b7f9c31d9e39d/Microsoft.NET.Workload.Android-11.0.100-ci.master.209.pkg)

iOS:

* Windows: [Microsoft.NET.Workload.iOS.14.1.100-ci.main.52.msi](https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/main/90bfb623ed14defe2475b5dd9040492c3f7048c9/492/package/Microsoft.NET.Workload.iOS.14.1.100-ci.main.52.msi)
* macOS: [Microsoft.iOS.Bundle.14.1.100-ci.main.52.pkg](https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/main/90bfb623ed14defe2475b5dd9040492c3f7048c9/492/package/Microsoft.iOS.Bundle.14.1.100-ci.main.52.pkg)

_NOTE: newer builds of .NET 5 *may* work, but your mileage may vary.
The workload installers enable a feature flag file via
`sdk/5.0.100-rtm.20509.5/EnableWorkloadResolver.sentinel`, which would
need to be created manually for other .NET 5 versions. You can find
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

    dotnet build HelloAndroid/HelloAndroid.csproj

You can launch the Android project to an attached emulator or device via:

    dotnet build HelloAndroid/HelloAndroid.csproj -t:Run

## iOS

Prerequisites:

* Xcode 12.1. Earlier versions won't work.

To build the iOS project:

    dotnet build HelloiOS/HelloiOS.csproj

To launch the iOS project on a simulator:

    dotnet build HelloiOS/HelloiOS.csproj -t:Run

## Xamarin.Forms

To launch the Forms project, you will need to specify a `$(TargetFramework)`:

    dotnet build HelloForms/HelloForms.csproj -t:Run -p:TargetFramework=net5.0-android
    dotnet build HelloForms/HelloForms.csproj -t:Run -p:TargetFramework=net5.0-ios

## Known Issues

Currently...

* There is not a way to setup a binding project for Xamarin.iOS.
* `System.Console.WriteLine` does not work on Xamarin.Android. Use
  `Android.Util.Log.Debug` for now.
* Building for device doesn't work for iOS.
* Building for tvOS or watchOS does not work.

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

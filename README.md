# net6-samples

_This is an *early* preview of Xamarin in .NET 6 **not for production use**. Expect breaking changes as Xamarin is still in development for .NET 6._

First install the [latest master build of .NET 6][0].

Projects:

* HelloAndroid - a native Xamarin.Android application
* HelloiOS - a native Xamarin.iOS application
* HelloForms, HelloForms.iOS, HelloForms.Android - a cross-platform Xamarin.Forms application

## Android

Prerequisites:

* You will need the Android SDK installed as well as `Android SDK Platform 29`. Simplest way to get this is to install the current Xamarin workload and go to `Tools > Android > Android SDK Manager` from within Visual Studio.

For example, to build the Android project:

    dotnet build HelloAndroid/HelloAndroid.csproj

You can launch the Android project to an attached emulator via:

    dotnet build HelloAndroid/HelloAndroid.csproj -t:Run

To deploy and run on a device, you can either modify `$(RuntimeIdentifier)` in
the `.csproj` or run:

    dotnet build HelloAndroid/HelloAndroid.csproj -t:Run -r android.21-arm64

## iOS

Prerequisites:

* Xcode 11.4. Earlier versions won't work.

To build the iOS project:

    dotnet build HelloiOS/HelloiOS.csproj

To launch the iOS project on a simulator:

    dotnet build HelloiOS/HelloiOS.csproj -t:Run

[0]: https://github.com/dotnet/installer#installers-and-binaries

## Known Issues

Currently...

* There is not a way to do `dotnet publish` with multiple RIDs.
* There is not a way to setup a binding project, neither for Xamarin.Android nor Xamarin.iOS.
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

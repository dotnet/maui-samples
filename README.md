# net5-samples

This is a preview of Xamarin in .NET 5.

First install the [latest master build of .NET 5][0].

Projects:

* HelloAndroid - a native Xamarin.Android application
* HelloiOS - a native Xamarin.iOS application
* HelloForms, HelloForms.iOS, HelloForms.Android - a cross-platform Xamarin.Forms application

For example, to build the Android project:

    dotnet publish HelloAndroid\HelloAndroid.csproj --self-contained

_TODO: a future build of Xamarin.Android will support deploying via:_

    dotnet publish -t:Install HelloAndroid\HelloAndroid.csproj --self-contained

To build the iOS project:

    dotnet publish HelloiOS/HelloiOS.csproj --self-contained

[0]: https://github.com/dotnet/installer#installers-and-binaries

## Workarounds

These are notes for things we currently had to workaround for these samples to work.

### NuGet

Currently, NuGet is not able to restore existing Xamarin.Android/iOS
packages for a .NET 5 project. We used `$(AssetTargetFallback)`,
however, this option does not work in combination with transitive
dependencies. The `Xamarin.AndroidX.*` set of NuGet packages has a
complex dependency tree. We just listed every package manually for
now.

Additionally, we had some problems with the Xamarin.Forms NuGet
package listing the same assembly in both:

* `lib\netstandard2.0\Xamarin.Forms.Platform.dll`
* `lib\MonoAndroid10.0\Xamarin.Forms.Platform.dll`

For now we added an MSBuild target in `Directory.Build.targets` to
resolve this. We also had to manually reference
`Xamarin.Forms.Platform.Android.dll`.

### AndroidX MSBuild tasks

We need to port some MSBuild tasks to `netstandard2.0` such as:

https://github.com/xamarin/AndroidSupportComponents/blob/68d28bc676673ec45f7f5ea2462c10bed87e2a2a/source/buildtasks/support-vector-drawable/Support-Vector-Drawable-BuildTasks.csproj#L10

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# net5-samples

This is a preview of Xamarin in .NET 5.

First install the [latest master build of .NET 5][0].

To build the Android project:

    dotnet publish HelloAndroid\HelloAndroid.csproj --self-contained

_TODO: a future build of Xamarin.Android will support deploying via:_

    dotnet publish -t:Install HelloAndroid\HelloAndroid.csproj --self-contained

To build the iOS project:

    dotnet publish HelloiOS/HelloiOS.csproj -r ios-x64 --self-contained true -p:PublishTrimmed=true -p:MonoAOT=true /p:UseAppHost=false

[0]: https://github.com/dotnet/installer#installers-and-binaries

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

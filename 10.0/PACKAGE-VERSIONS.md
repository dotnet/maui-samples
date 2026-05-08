# Centralized Package Versions

This repository uses a [`Directory.Build.props`](Directory.Build.props) file to manage NuGet package versions centrally for all samples in this folder. This means individual `.csproj` files reference version variables like `$(MauiVersion)` instead of hardcoded version numbers.

## How it works

The `Directory.Build.props` at the root of this folder defines:

```xml
<MauiVersion>10.0.60</MauiVersion>    <!-- Microsoft.Maui.Controls -->
<DotNetVersion>10.0.7</DotNetVersion>  <!-- Microsoft.Extensions.* packages -->
```

Each project then references these:

```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="$(DotNetVersion)" />
```

## In your own projects

**This pattern is specific to this samples repository** to make it easy to update 100+ projects at once. In your own apps, use explicit version numbers directly in your `.csproj`:

```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="10.0.60" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.7" />
```

You can find the latest versions on [NuGet.org](https://www.nuget.org/packages/Microsoft.Maui.Controls).

## Automatic updates

This repository uses [Dependabot](https://docs.github.com/en/code-security/dependabot) to automatically open PRs when new stable versions are available. See [`.github/dependabot.yml`](../.github/dependabot.yml) for the configuration.

## Learn more

- [Manage .NET project SDKs - Microsoft Learn](https://learn.microsoft.com/dotnet/core/project-sdk/overview)
- [Directory.Build.props - Microsoft Learn](https://learn.microsoft.com/visualstudio/msbuild/customize-by-directory)

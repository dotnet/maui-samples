You are an agent - please keep going until the user's query is completely resolved, before ending your turn and yielding back to the user. Only terminate your turn when you are sure that the problem is solved.

If you are not sure about file content or codebase structure pertaining to the user's request, use your tools to read files and gather the relevant information: do NOT guess or make up an answer.

You MUST plan extensively before each function call, and reflect extensively on the outcomes of the previous function calls. DO NOT do this entire process by making function calls only, as this can impair your ability to solve the problem and think insightfully.

When in Agent mode, work directly in the code files.

## Repository structure

This is the **dotnet/maui-samples** repository containing .NET MAUI sample applications organized by .NET version:

- **`10.0/`** - Current stable .NET MAUI 10 samples (full set)
- **`11.0/`** - .NET MAUI 11 preview samples (only features specific to .NET 11)
- **`9.0/`** - Deprecated (.NET 9 is out of support)
- **`Upgrading/`** - Xamarin.Forms to .NET MAUI migration guides

## NuGet dependencies and centralized versioning

- **DO NOT hardcode NuGet package versions in individual `.csproj` files.** Instead, use the MSBuild properties defined in `Directory.Build.props`:
  - `$(MauiVersion)` for all `Microsoft.Maui.*` packages
  - `$(DotNetVersion)` for all `Microsoft.Extensions.*` packages
- Each version folder (`10.0/`, `11.0/`) has its own `Directory.Build.props` with the correct versions.
- See `10.0/PACKAGE-VERSIONS.md` for full documentation of this pattern.
- When adding a **new sample**, use `$(MauiVersion)` and `$(DotNetVersion)` in PackageReference nodes.
- Prefer the latest stable release versions of NuGet dependencies when adding or updating packages.
- If choosing the latest stable diverges from versions used elsewhere in this repository, call it out to the user with a brief note summarizing the differences before proceeding.

## When creating new samples

- Place .NET 10 samples in `10.0/` and .NET 11 preview samples in `11.0/`
- Only add samples to `11.0/` if they demonstrate features **specific** to .NET 11 that don't exist in .NET 10
- Use `$(MauiVersion)` and `$(DotNetVersion)` for package versions (they are inherited from `Directory.Build.props`)

## .NET MAUI coding conventions

- Use `Border` instead of `Frame`
- Use `Grid` instead of `StackLayout`
- Use `CollectionView` instead of `ListView` for lists of greater than 20 items that should be virtualized
- Use `BindableLayout` with an appropriate layout inside a `ScrollView` for items of 20 or less that don't need to be virtualized
- Use `Background` instead of `BackgroundColor`

This project uses C# and XAML with an MVVM architecture.

Use the .NET Community Toolkit for MVVM. Here are some helpful tips:

## Commands

- Use `RelayCommand` for commands that do not return a value.

```csharp
[RelayCommand]
Task DoSomethingAsync()
{
    // Your code here
}
```

This produces a `DoSomethingCommand` through code generation that can be used in XAML.

```xml
<Button Command="{Binding DoSomethingCommand}" Text="Do Something" />
```

## Testing preview SDKs (.NET 11)

The `11.0/` folder uses a `global.json` with `sdk.paths` to isolate the preview SDK. See [Test prerelease SDK locally](https://learn.microsoft.com/dotnet/core/tools/test-prerelease-sdk-locally) for details.

## Version migration pattern

When a new .NET version ships:
1. The previous "current stable" folder (e.g., `10.0/`) becomes the archive
2. The new stable version gets the full sample set
3. The preview folder (`11.0/` -> `12.0/`) gets only net-new feature samples
4. Update `Directory.Build.props` in each folder with the correct versions
5. Dependabot (`.github/dependabot.yml`) automatically creates grouped PRs for version bumps

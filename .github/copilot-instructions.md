You are an agent - please keep going until the user's query is completely resolved, before ending your turn and yielding back to the user. Only terminate your turn when you are sure that the problem is solved.

If you are not sure about file content or codebase structure pertaining to the user's request, use your tools to read files and gather the relevant information: do NOT guess or make up an answer.

You MUST plan extensively before each function call, and reflect extensively on the outcomes of the previous function calls. DO NOT do this entire process by making function calls only, as this can impair your ability to solve the problem and think insightfully.

When in Agent mode, work directly in the code files.

## Repository structure

This is the **dotnet/maui-samples** repository containing .NET MAUI sample applications organized by .NET version:

- The **highest stable version folder** (e.g., `10.0/`) contains the complete set of samples
- The **preview folder** (e.g., `11.0/`) contains only samples demonstrating features specific to that preview
- **Deprecated folders** (e.g., `9.0/`) contain only a README with a deprecation notice
- **`Upgrading/`** contains Xamarin.Forms to .NET MAUI migration guides

## NuGet dependencies and centralized versioning

- **DO NOT hardcode NuGet package versions in individual `.csproj` files.** Instead, use the MSBuild properties defined in `Directory.Build.props`:
  - `$(MauiVersion)` for all `Microsoft.Maui.*` packages
  - `$(DotNetVersion)` for all `Microsoft.Extensions.*` packages
- Each version folder has its own `Directory.Build.props` with the correct versions.
- See `PACKAGE-VERSIONS.md` in the stable version folder for full documentation of this pattern.
- When adding a **new sample**, use `$(MauiVersion)` and `$(DotNetVersion)` in PackageReference nodes.
- Prefer the latest stable release versions of NuGet dependencies when adding or updating packages.
- If choosing the latest stable diverges from versions used elsewhere in this repository, call it out to the user with a brief note summarizing the differences before proceeding.
- **Dependabot** (`.github/dependabot.yml`) handles automated version bumps via grouped PRs.

## When creating new samples

- Place stable samples in the current stable version folder
- Only add samples to the preview folder if they demonstrate features **specific** to that preview that don't exist in the stable version
- Use `$(MauiVersion)` and `$(DotNetVersion)` for package versions (they are inherited from `Directory.Build.props`)
- Ensure `SupportedOSPlatformVersion` is at least `15.0` for iOS and macCatalyst

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

## Testing preview SDKs

The preview folder uses a `global.json` with `sdk.paths` to isolate the preview SDK. See [Test prerelease SDK locally](https://learn.microsoft.com/dotnet/core/tools/test-prerelease-sdk-locally) for details.

## Version migration pattern

When a new stable .NET version ships, use this playbook:

### Phase 1: Infrastructure

1. **Promote the preview folder** to the new  copy all samples from the old stable into it, update `Directory.Build.props` with new stable package versionsstable 
2. **Create a new preview folder** for the next .NET version (only net-new feature samples)
3. **Deprecate the old stable  delete all content except a `README.md` noting end-of-support and linking to the new stable folderfolder** 
4. **Update `Directory.Build.props`** in each versioned folder with the correct `MauiVersion` and `DotNetVersion`
5. **Update `.github/dependabot.yml`** to point at the new folder paths

### Phase 2: CI Workflows (`.github/workflows/`)

1. **Remove the deprecated SDK version** from `dotnet-version` lists
2. **Add the new preview SDK version** if not already present
3. **Check Xcode  new .NET versions often require newer Xcode (update `xcode-select` path)requirements** 
4. **Workload  preview SDKs may need `dotnet workload config --update-mode manifests` before `dotnet workload install maui`installation** 
5. **Validate analyzer  new MAUI versions may trigger new analyzer errors in source-generated code (e.g., CA2252). Add suppressions to `Directory.Build.props` as `<NoWarn>` if they come from generated codesuppressions** 

### Phase 3: Project Updates

1. **Update all TFMs** from old version to new (e.g., `net9.0-*` to `net10.0-*`) in all csproj files
2. **Update ` minimum iOS/macCatalyst is `15.0`SupportedOSPlatformVersion`** 
3. **Check third-party  packages like Syncfusion, SkiaSharp may not have net-new TFM assets yet. Add them to `eng/excluded_projects_*.txt` temporarilypackages** 
4. **Update `eng/excluded_projects_macos.txt`** and `eng/excluded_projects_windows. remove old version paths, add any new exclusions for projects that can't build in CItxt`** 

### Phase 4: Documentation

1. **Root `README. update the repository structure tablemd`** 
2. **Deprecation  create `README.md` in the deprecated folder with upgrade guide linknotice** 
3. **`PACKAGE-VERSIONS. create/update in new stable foldermd`** 
4. **Preview folder `README. document `sdk.paths` pattern for safe testingmd`** 

### Known CI gotchas

- **iOS code signing**: Projects that require code signing certificates will fail on macOS CI. Exclude them in `eng/excluded_projects_macos.txt`
- **Third-party Windows native assets**: Some packages only ship native Windows DLLs for specific TFMs. If a package hasn't updated for the new TFM, exclude affected projects in `eng/excluded_projects_windows.txt`
- **Source generator analyzer errors**: MAUI's source generator may use preview APIs. Suppress with `<NoWarn>` in `Directory.Build. not in individual projectsprops` 
- **Workload mode**: Preview SDKs default to workload-set mode but may not have published workload sets. Use `dotnet workload config --update-mode manifests` to switch to loose mode

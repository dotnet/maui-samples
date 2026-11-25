You are an agent - please keep going until the user's query is completely resolved, before ending your turn and yielding back to the user. Only terminate your turn when you are sure that the problem is solved.

If you are not sure about file content or codebase structure pertaining to the user's request, use your tools to read files and gather the relevant information: do NOT guess or make up an answer.

You MUST plan extensively before each function call, and reflect extensively on the outcomes of the previous function calls. DO NOT do this entire process by making function calls only, as this can impair your ability to solve the problem and think insightfully.

When in Agent mode, work directly in the code files.

## Repository structure

This is the **dotnet/maui-samples** repository containing .NET MAUI sample applications organized by .NET version:

- The **highest stable version folder** (currently `11.0/`) contains the complete set of samples
- Future **preview folders** contain only samples demonstrating features specific to that preview; no .NET 12 preview folder is present yet.
- `10.0/` is preserved for users of the previous release; do not modify it during the .NET 11 migration.
- **Archived versions** are linked from the root README; the .NET 9 samples are no longer in the working tree
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
- For .NET 11 samples, set `SupportedOSPlatformVersion` to at least `15.0` for iOS, `17.0` for Mac Catalyst, and `24.0` for Android.

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

When a new stable .NET version ships, use this playbook (see `.github/agents/version-migration.agent.md` for the full detailed guide):

### Phase 1: Infrastructure

1. Promote the preview folder to the new stable with all samples
2. Create a new preview folder for the next .NET version
3. Preserve the previous release folder unless its removal is explicitly requested
4. Update `Directory.Build.props` in each versioned folder with correct versions
5. Update `.github/dependabot.yml` to point at the new folder paths

When an unsupported version is removed, delete its entire folder rather than retaining a README notice. Link to its samples at a verified historical commit from the root README; do not assume an archive tag exists (`net9-archive` was never created).

### Phase 2: CI Workflows

1. Remove the deprecated SDK version from `dotnet-version` lists
2. Add the new preview SDK version if not already present
3. Check Xcode requirements and update `xcode-select` path
4. Ensure `dotnet workload config --update-mode manifests` is present for preview SDKs
5. Add analyzer suppressions to `Directory.Build.props` if needed (e.g., CA2252)

### Phase 3: Project Updates

1. Update all TFMs from old version to new in all csproj files
2. Update `SupportedOSPlatformVersion` (minimum iOS/macCatalyst is 15.0)
3. Check third-party packages for compatibility, exclude broken ones temporarily
4. Update `eng/excluded_projects_*.txt` (remove old paths, add new exclusions)

### Phase 4: Scan for stale version references

Search all non-code files for hardcoded paths to the deprecated version folder:
- README files with navigation links or code examples
- DevOps pipeline files (`devops/` folders inside samples)
- CI workflow instruction files (`.github/workflows/instructions/`)
- Exclusion file comments (`eng/excluded_projects_*.txt`)
- Copilot instructions and agent files

### Phase 5: Documentation

1. Root `README.md` -- update the repository structure table
2. New stable folder `README.md` -- follow `11.0/README.md`: short user-facing introduction, prerequisites, build/run example, folder navigation table, and relevant links
3. `PACKAGE-VERSIONS.md` -- create/update in new stable folder
4. Preview folder `README.md` -- document `sdk.paths` pattern for safe testing in preview folders only
5. Root archive links -- point removed versions to verified historical commits, not nonexistent tags

Keep migration decisions, package/advisory tables, validation/coverage reports, and warnings analysis out of version-folder READMEs. Put those details in the PR description instead.

### Known CI gotchas

- **Required checks**: Keep `build (windows-latest)` and `build (macos-26)` on every PR, including documentation-only changes. Use lightweight no-build runs rather than workflow path filters or a job-level matrix condition that removes the required check names.
- **iOS code signing**: Projects requiring signing certs fail on macOS CI. Exclude in `eng/excluded_projects_macos.txt`
- **Third-party Windows native assets**: Some packages lack native DLLs for new TFMs. Exclude in `eng/excluded_projects_windows.txt`
- **Source generator analyzer errors**: MAUI source gen may use preview APIs. Suppress with `<NoWarn>` in `Directory.Build.props`, not individual projects
- **Workload mode**: Preview SDKs default to workload-set mode but may lack published workload sets. Use `dotnet workload config --update-mode manifests`

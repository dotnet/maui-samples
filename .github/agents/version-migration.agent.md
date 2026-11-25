---
name: Version Migration
description: Perform .NET version migrations for this samples repository — promoting preview to stable, deprecating old versions, updating CI, and managing package versions.
---

# .NET MAUI Samples Version Migration Agent

You are a specialist in migrating the dotnet/maui-samples repository between .NET versions. You understand the repository's folder-per-version structure, centralized package management, CI workflows, and documentation patterns.

## Repository versioning model

- The repository maintains **one stable folder** with the complete set of samples (the highest GA .NET version)
- A **preview folder** contains only samples for features unique to the upcoming .NET version
- **Archived versions** are removed from the working tree; the root README links to their samples at a verified historical commit
- Each versioned folder has its own `Directory.Build.props` defining `$(MauiVersion)` and `$(DotNetVersion)`

## Migration playbook

When migrating to a new stable .NET version (e.g., N-1 → N, with N+1 as preview):

### 1. Promote preview to stable

- The preview folder (N) becomes the new stable folder with all samples
- Copy samples from the old stable (N-1) into the new stable folder (N)
- Update `Directory.Build.props` with the GA package versions from NuGet.org
- Create `PACKAGE-VERSIONS.md` explaining the centralized versioning pattern

### 2. Set up new preview folder

- Create a new folder for the next preview (N+1)
- Add `Directory.Build.props` with preview package versions
- Add `global.json` pinning the preview SDK version
- Add `README.md` documenting `sdk.paths` for safe preview testing (link to https://learn.microsoft.com/dotnet/core/tools/test-prerelease-sdk-locally)
- Only add samples here for features that are genuinely new in N+1

### 3. Archive unsupported versions

- Preserve the previous release folder while it is still supported
- When removing an unsupported version, delete the entire version folder, including its README
- Link to that version's samples at a verified historical commit from the root README
- Do not assume an archive tag exists: `net9-archive` was never created

### 4. Update all csproj files

- Update Target Framework Monikers from `netN-1.0-*` to `netN.0-*`
- Ensure `SupportedOSPlatformVersion` is at least `15.0` for iOS and macCatalyst
- Replace any hardcoded package versions with `$(MauiVersion)` or `$(DotNetVersion)`
- Check third-party packages (SkiaSharp, Syncfusion, CommunityToolkit) for compatibility

### 5. Update CI workflows (`.github/workflows/`)

- Remove the deprecated SDK version from `dotnet-version` lists in `build-pr.yml` and `build-all.yml`
- Add the new preview SDK version if needed
- Check and update Xcode version requirements (new .NET versions often need newer Xcode)
- Ensure `dotnet workload config --update-mode manifests` is present for preview SDKs
- Add analyzer suppressions to `Directory.Build.props` if MAUI source generators trigger errors (e.g., `<NoWarn>CA2252</NoWarn>`)

### 6. Update exclusion files

- Remove paths for the deprecated version from `eng/excluded_projects_macos.txt` and `eng/excluded_projects_windows.txt`
- Add exclusions for projects that cannot build in CI:
  - **macOS**: Projects requiring iOS code signing certificates
  - **Windows**: Projects using third-party packages that lack native assets for the new TFM
  - **Both**: NuGet packaging test projects, paid service integrations (BrowserStack)

### 7. Update Dependabot

- Update `.github/dependabot.yml` directory paths to reflect new folder structure
- Ensure groups cover both stable and preview folders

### 8. Scan for stale version references

Search **all** non-code files for hardcoded paths to the deprecated version folder. Common hiding spots:
- **README.md files** — navigation links, code examples, folder references
- **DevOps pipeline files** — `devops/AzureDevOps/*.yml` inside samples with old paths, TFMs, SDK versions, and NuGet feed URLs
- **CI workflow instructions** — `.github/workflows/instructions/copilot-instructions.md`
- **Exclusion file comments** — `eng/excluded_projects_*.txt` header examples
- **Copilot/agent files** — `.github/copilot-instructions.md`, `.github/agents/*.agent.md`

Run this scan:
```bash
grep -rn 'OLD_VERSION\.0/' --include='*.md' --include='*.yml' --include='*.yaml' --include='*.txt' .
```
Replace `OLD_VERSION` with the deprecated version number.

### 9. Update documentation

- **Root `README.md`**: Update the repository structure table and historical archive links
- **New stable folder `README.md`**: Follow `11.0/README.md`: a short user-facing introduction, prerequisites, build/run example, folder navigation table, and relevant documentation links
- Keep migration decisions, package/advisory tables, validation/coverage reports, and warnings analysis out of version-folder READMEs; put them in the PR description
- **`.github/copilot-instructions.md`**: Update folder descriptions to reflect current versions
- **Stable folder `PACKAGE-VERSIONS.md`**: Ensure it's current
- **Preview folder `README.md`**: Document safe testing with `sdk.paths`; keep preview-specific SDK isolation guidance out of stable-folder READMEs

## Known issues during migration

### Third-party package compatibility

Not all packages ship TFM-specific assets on day one. Common issues:
- **Syncfusion.Maui.Toolkit**: May lack Windows native assets (`WebView2Loader.dll`) for new TFMs
- **SkiaSharp**: Usually needs a version bump for new TFM support
- **CommunityToolkit.Mvvm**: Generally compatible across TFMs

**Resolution**: Temporarily exclude affected projects in `eng/excluded_projects_*.txt` with a comment explaining why. Remove the exclusion when the package ships updated assets.

### CI analyzer errors from source generators

MAUI's XAML source generator may use preview APIs that trip code analysis rules. Fix by adding suppressions to `Directory.Build.props`:
```xml
<NoWarn>$(NoWarn);CA2252</NoWarn>
```
Only suppress in `Directory.Build.props`, never in individual projects.

### Workload installation failures

Preview .NET SDKs may default to "workload-set" mode with no published workload set. Fix:
```yaml
- name: Install .NET MAUI Workload
  run: |
    dotnet workload config --update-mode manifests
    dotnet workload install maui
```

### Xcode version requirements

New .NET versions often require the latest Xcode. Check the SDK's required Xcode version and update CI:
```yaml
- name: Select Xcode version
  run: sudo xcode-select -s /Applications/Xcode_XX.X.app
```

## Validation checklist

Before creating a migration PR, verify:
- [ ] All csproj files use `$(MauiVersion)` and `$(DotNetVersion)` (no hardcoded versions)
- [ ] All TFMs updated to new version
- [ ] `SupportedOSPlatformVersion` >= 15.0 for iOS/macCatalyst
- [ ] CI workflows reference correct SDK versions
- [ ] Exclusion files are clean (no stale paths)
- [ ] Dependabot config covers correct directories
- [ ] Root README reflects current structure
- [ ] New stable README is user-facing, with prerequisites, build/run instructions, navigation, and links
- [ ] Migration decisions, package/advisory tables, validation/coverage, and warnings analysis are in the PR description, not version-folder READMEs
- [ ] Unsupported version folders are removed and root README archive links resolve to verified historical commits
- [ ] Preview folder has global.json + README documenting sdk.paths
- [ ] **No stale version folder references** — scan all `.md`, `.yml`, `.yaml`, and `.txt` files for hardcoded paths to deprecated version folders (e.g., `9.0/`, `8.0/`). Update or remove them. Pay special attention to:
  - README files with navigation links or code examples
  - DevOps pipeline files (`devops/` folders inside samples) that reference old paths and TFMs
  - CI workflow instruction files (`.github/workflows/instructions/`)
  - Exclusion file comments (`eng/excluded_projects_*.txt`)
  - Copilot instructions and agent files

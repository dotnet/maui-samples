---
mode: agent
---

## Objective
Migrate all .NET MAUI samples from the 9.0 folder to .NET 10, ensuring compatibility, removing deprecated APIs, and following latest best practices.

## Prerequisites
- Access to Microsoft Learn MCP for documentation lookups
- .NET 10 SDK installed and workloads configured
- Understanding of .NET MAUI deprecation patterns from .NET 9 to .NET 10

## Migration Steps

### Step 1: Prepare Directory Structure
**Action:** Create the 10.0 folder structure mirroring 9.0 organization

```

# Create 10.0 root folder if it doesn't exist

New-Item -Path "10.0" -ItemType Directory -Force

# List all sample categories in 9.0

Get-ChildItem -Path "9.0" -Directory

```

**Verification:**
- Confirm 10.0 folder exists
- Document all sample categories found in 9.0

### Step 2: Copy Sample Projects (Excluding Build Artifacts)
**Action:** Copy all samples from 9.0 to 10.0, excluding build artifacts

```


# For each sample in 9.0

Get-ChildItem -Path "9.0" -Recurse -Directory | ForEach-Object {
\$sourcePath = \$_.FullName
\$targetPath = \$sourcePath -replace '\\9.0\\', '\\10.0\\'

    # Copy excluding bin, obj, and .vs folders
    robocopy $sourcePath $targetPath /E /XD bin obj .vs /NFL /NDL /NJH /NJS
    }

```

**Verification:**
- Confirm all samples copied successfully
- Verify no bin/obj folders were copied
- Check that solution and project files are present

### Step 3: Update Target Framework Monikers (TFMs)
**Action:** Update all .csproj files to target .NET 10

**Find and Replace:**
- `net9.0-android` → `net10.0-android`
- `net9.0-ios` → `net10.0-ios`
- `net9.0-maccatalyst` → `net10.0-maccatalyst`
- `net9.0-windows` → `net10.0-windows10.0.19041.0`
- `net9.0-tizen` → `net10.0-tizen` (if present)

```


# Batch update all .csproj files

Get-ChildItem -Path "10.0" -Recurse -Filter "*.csproj" | ForEach-Object {
\$content = Get-Content \$_.FullName -Raw
\$content = \$content -replace 'net9\.0-', 'net10.0-'
Set-Content \$_.FullName -Value \$content -NoNewline
Write-Host "Updated: $($_.FullName)"
}

```

**Verification:**
- Use Microsoft Learn MCP to verify correct .NET 10 TFMs
- Confirm no net9.0 references remain in any .csproj

### Step 4: Update SupportedOSPlatformVersion
**Action:** Update minimum OS versions to .NET 10 recommendations

**Use Microsoft Learn MCP to query:**
- "What are the recommended SupportedOSPlatformVersion values for .NET MAUI 10?"

**Expected Updates:**
```

<!-- Android -->
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>

<!-- iOS -->
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>

<!-- MacCatalyst -->
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.0</SupportedOSPlatformVersion>

<!-- Windows -->
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.19041.0</SupportedOSPlatformVersion>
<TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.19041.0</TargetPlatformMinVersion>

```

**Verification:**
- Cross-reference with Microsoft Learn MCP
- Document any version changes from .NET 9

### Step 5: Update NuGet Package Versions
**Action:** Update all MAUI and related packages to latest .NET 10 compatible versions

**Query Microsoft Learn MCP:**
- "What is the latest Microsoft.Maui.Controls version for .NET 10?"
- "What is the latest CommunityToolkit.Maui version compatible with .NET 10?"

**Expected Package Updates:**
```

<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.*" />
<PackageReference Include="CommunityToolkit.Maui" Version="10.*" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
```

```


# List all packages that need updating

Get-ChildItem -Path "10.0" -Recurse -Filter "*.csproj" | ForEach-Object {
Select-String -Path \$_.FullName -Pattern '<PackageReference Include="Microsoft\.'
}

```

**Verification:**
- Check NuGet.org for latest stable versions
- Verify compatibility with .NET 10
- Document any breaking changes in package updates

### Step 6: Scan and Update Deprecated APIs
**Action:** Identify and replace all deprecated APIs using Microsoft Learn MCP

**Critical Deprecated APIs to Find:**

#### 6.1 Device Class (Fully Deprecated)
```


# Find all Device class usages

Get-ChildItem -Path "10.0" -Recurse -Include "*.cs","*.xaml" |
Select-String -Pattern "Device\." |
Select-Object Path, LineNumber, Line

```

**Replacements:**
- `Device.RuntimePlatform` → `DeviceInfo.Platform`
- `Device.Idiom` → `DeviceInfo.Idiom`
- `Device.InvokeOnMainThreadAsync()` → `MainThread.InvokeOnMainThreadAsync()`
- `Device.GetNamedSize()` → Use explicit numeric values (e.g., `18`)

**Query MCP:** "What are all the deprecated Device class APIs in .NET MAUI 10 and their replacements?"

#### 6.2 Frame Control (Obsolete)
```


# Find all Frame usages

Get-ChildItem -Path "10.0" -Recurse -Filter "*.xaml" |
Select-String -Pattern "<Frame " |
Select-Object Path, LineNumber

```

**Replacement:**
```

<!-- BEFORE -->
<Frame BorderColor="Gray" CornerRadius="10" Padding="20">
    <Label Text="Content"/>
</Frame>
<!-- AFTER -->
<Border Stroke="Gray" StrokeShape="RoundRectangle 10" Padding="20">
    <Label Text="Content"/>
</Border>
```

#### 6.3 AndExpand Layout Options (Deprecated)
```


# Find all AndExpand usages

Get-ChildItem -Path "10.0" -Recurse -Include "*.cs","*.xaml" |
Select-String -Pattern "AndExpand" |
Select-Object Path, LineNumber, Line

```

**Replacement:**
- Remove `AndExpand` suffix from all LayoutOptions
- Use `Grid` with star-sized rows/columns for expansion behavior

#### 6.4 Application.MainPage (Obsolete)
```


# Find MainPage property usage

Get-ChildItem -Path "10.0" -Recurse -Filter "*.cs" |
Select-String -Pattern "MainPage\s*=" |
Select-Object Path, LineNumber

```

**Replacement:**
```

// BEFORE
public App()
{
InitializeComponent();
MainPage = new AppShell();
}

// AFTER
public App()
{
InitializeComponent();
}

protected override Window CreateWindow(IActivationState? activationState)
{
return new Window(new AppShell());
}

```

#### 6.5 AutomationProperties (Deprecated)
```


# Find AutomationProperties usage

Get-ChildItem -Path "10.0" -Recurse -Filter "*.xaml" |
Select-String -Pattern "AutomationProperties\." |
Select-Object Path, LineNumber

```

**Replacement:**
- `AutomationProperties.Name` → `SemanticProperties.Description`
- `AutomationProperties.HelpText` → `SemanticProperties.Hint`

#### 6.6 MessagingCenter (Deprecated)
```


# Find MessagingCenter usage

Get-ChildItem -Path "10.0" -Recurse -Filter "*.cs" |
Select-String -Pattern "MessagingCenter\." |
Select-Object Path, LineNumber

```

**Replacement:**
- Use `WeakReferenceMessenger` from `CommunityToolkit.Mvvm`
- Add package: `CommunityToolkit.Mvvm`

**Query MCP:** "How do I migrate from MessagingCenter to WeakReferenceMessenger in .NET MAUI 10?"

### Step 7: Fix Grid Layout Definitions
**Action:** Ensure all Grid controls have explicit RowDefinitions and ColumnDefinitions

```


# Find potential Grid layout issues (grids using Grid.Row without definitions)

Get-ChildItem -Path "10.0" -Recurse -Filter "*.xaml" | ForEach-Object {
\$content = Get-Content $_.FullName -Raw
    if ($content -match '<Grid[^>]*>' -and \$content -match 'Grid\.Row=' -and \$content -notmatch 'Grid\.RowDefinitions') {
Write-Warning "Potential missing RowDefinitions in: $($_.FullName)"
}
}

```

**Verification:**
- Manually review flagged files
- Add explicit RowDefinitions/ColumnDefinitions where missing

### Step 8: Address Build Warnings
**Action:** Build each sample and resolve warnings using Microsoft Learn MCP

```


# Build all samples and capture warnings

Get-ChildItem -Path "10.0" -Recurse -Filter "*.csproj" | ForEach-Object {
Write-Host "`nBuilding: $($_.Name)" -ForegroundColor Cyan
dotnet build \$_.FullName -c Release 2>\&1 | Tee-Object -FilePath "build-log.txt" -Append
}

```

**For each warning:**
1. Copy the warning message
2. Query Microsoft Learn MCP: "How do I resolve this .NET MAUI 10 warning: [paste warning]"
3. Apply recommended fix
4. Document the fix in a migration notes file

**Common Warnings to Expect:**
- CS0618: Obsolete API usage
- MAUI0001: Frame is deprecated
- MAUI0002: AndExpand layout options are deprecated

### Step 9: Update Image Resource Naming
**Action:** Ensure all image files follow .NET MAUI naming requirements

**Requirements:**
- Lowercase only
- Start and end with letter
- Only alphanumeric or underscores
- No spaces, hyphens, or special characters

```


# Find non-compliant image names

Get-ChildItem -Path "10.0" -Recurse -Include "*.png","*.jpg","*.svg" | Where-Object {
\$_.Name -cmatch '[A-Z]' -or \$_.Name -match '[ -]' -or \$_.Name -match '^[0-9]'
} | ForEach-Object {
\$newName = \$_.Name.ToLower() -replace '[^a-z0-9._]', '_'
Write-Host "Rename: $($_.Name) → \$newName"
\# Rename-Item \$_.FullName -NewName \$newName
}

```

### Step 10: Validate and Test
**Action:** Comprehensive testing of migrated samples

#### 10.1 Build Validation
```


# Clean build all samples

Get-ChildItem -Path "10.0" -Recurse -Filter "*.csproj" | ForEach-Object {
dotnet clean \$_.FullName
dotnet build \$_.FullName -c Release --no-incremental
}

```

**Success Criteria:**
- All samples build without errors
- Zero deprecated API warnings (CS0618)
- Zero MAUI-specific warnings

#### 10.2 Runtime Testing
For each sample category:
- Run on at least one Android emulator/device
- Run on at least one iOS simulator/device (if on macOS)
- Verify core functionality works
- Check for runtime exceptions

#### 10.3 Documentation Review
- Verify README.md is still accurate
- Update any .NET 9 references to .NET 10
- Document any breaking changes

### Step 11: Create Migration Summary
**Action:** Document all changes made during migration

Create `10.0/MIGRATION_NOTES.md`:

```


# .NET 9 to .NET 10 Migration Summary

## Date: [Current Date]

### Samples Migrated

- Total samples: [count]
- Categories: [list categories]


### Framework Updates

- TFM: net9.0 → net10.0
- SupportedOSPlatformVersion changes: [document changes]


### Package Updates

| Package | .NET 9 Version | .NET 10 Version |
| :-- | :-- | :-- |
| Microsoft.Maui.Controls | [version] | [version] |
| CommunityToolkit.Maui | [version] | [version] |

### Deprecated API Replacements

- Device.RuntimePlatform → DeviceInfo.Platform ([count] instances)
- Frame → Border ([count] instances)
- AndExpand removed ([count] instances)
- [list all others]


### Build Warnings Resolved

- [Document all warnings and resolutions]


### Known Issues

- [Document any unresolved issues]


### Testing Status

- [x] All samples build successfully
- [x] Android testing completed
- [ ] iOS testing completed (if applicable)
- [ ] Windows testing completed (if applicable)

```

### Step 12: Final Verification Checklist

Before considering migration complete:

- [ ] No `net9.0` references remain in any .csproj file
- [ ] All `Device.` class usages replaced
- [ ] All `Frame` controls replaced with `Border`
- [ ] All `AndExpand` layout options removed
- [ ] All `Application.MainPage` usages replaced with `CreateWindow()`
- [ ] All `AutomationProperties` replaced with `SemanticProperties`
- [ ] All image file names comply with naming rules
- [ ] All samples build without warnings
- [ ] All samples tested on at least one platform
- [ ] Migration notes documentation completed
- [ ] README files reviewed and updated

## Microsoft Learn MCP Query Strategy

Use these structured queries to get targeted help:

1. **Before starting:** "What are the breaking changes between .NET MAUI 9 and .NET MAUI 10?"

2. **For TFM updates:** "What are the correct TargetFrameworks and SupportedOSPlatformVersion values for .NET MAUI 10?"

3. **For deprecated APIs:** "What APIs were deprecated or removed in .NET MAUI 10 and what are their replacements?"

4. **For specific warnings:** "How do I resolve [specific warning code and message] in .NET MAUI 10?"

5. **For package compatibility:** "Is [package name] version [version] compatible with .NET MAUI 10?"

## Troubleshooting Common Issues

### Issue: Build fails with TFM error
**Query MCP:** "What is the correct TargetFramework syntax for .NET MAUI 10?"

### Issue: Package restore fails
**Query MCP:** "What is the latest stable version of [package name] for .NET MAUI 10?"

### Issue: Deprecated API warning
**Query MCP:** "What replaced [deprecated API] in .NET MAUI 10?"

### Issue: Layout rendering incorrectly
**Review:** Grid RowDefinitions/ColumnDefinitions are explicit

## Success Criteria

Migration is complete when:
1. All samples build with zero errors
2. All samples build with zero warnings
3. All deprecated APIs have been replaced
4. All samples run successfully on target platforms
5. Migration documentation is complete
6. Code review passes quality checks

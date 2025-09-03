---
name: "Upgrade MAUI Samples"
about: "Request scripted upgrade of .NET MAUI sample folders from one major version to another"
title: ".NET MAUI Sample Upgrade Guide"
labels: [upgrade, automated]
assignees: []
---

# Upgrade MAUI Samples

## Start

To upgrade .NET MAUI samples from one version to another, run this single command:

```powershell
pwsh ./.github/scripts/automated-sample-upgrade.ps1 -SourceVersion 8.0 -TargetVersion 10.0
```

## After Running the Script

**IMPORTANT**: The script will display AI instructions. Follow them exactly:

## Strict Constraints

❌ **DO NOT**:
- Make style changes or refactoring
- Update package versions (unless required for deprecations)
- Modify project structure
- Add new features
- Touch the source folder (e.g., 8.0)

✅ **DO ONLY**:
- Fix deprecated/obsolete APIs found in Microsoft documentation
- Work only in the target folder

## Expected Outcome

After following the guidance:
- All samples use the correct target framework
- Deprecated/obsolete APIs are updated to current equivalents
- No unnecessary changes are made
- Source samples remain untouched as reference

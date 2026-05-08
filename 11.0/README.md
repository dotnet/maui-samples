# .NET MAUI 11 Preview Samples

This folder contains samples that demonstrate **features specific to .NET MAUI 11** (currently in preview). For the full set of .NET MAUI samples, see the [10.0/](../10.0/) folder.

## Testing preview SDKs safely

.NET 10+ supports [`sdk.paths`](https://learn.microsoft.com/dotnet/core/tools/test-prerelease-sdk-locally) in `global.json`, which lets you install a preview SDK into a project-local folder without modifying your system-wide installation.

### Quick start

```bash
# 1. Install the preview SDK locally (does NOT modify your system PATH)
./dotnet-install.sh --version 11.0.100-preview.3.26207.106 --install-dir ./.dotnet

# 2. The global.json in this folder already has sdk.paths configured:
cat global.json
```

The `global.json` in this folder uses `sdk.paths` to point to a local `.dotnet/` directory:

```json
{
  "sdk": {
    "version": "11.0.100-preview.3.26207.106",
    "paths": [".dotnet"]
  }
}
```

This means:
- Your system-wide `dotnet` host (must be .NET 10+) reads this file
- It finds the preview SDK in `./.dotnet/` and uses it for this project only
- No other projects on your machine are affected
- To undo: just delete the `.dotnet/` folder

### Learn more

- [Test prerelease SDK locally - Microsoft Learn](https://learn.microsoft.com/dotnet/core/tools/test-prerelease-sdk-locally)
- [global.json overview - Microsoft Learn](https://learn.microsoft.com/dotnet/core/tools/global-json)

## Centralized package versions

Like the `10.0/` folder, this folder uses a [`Directory.Build.props`](Directory.Build.props) to manage NuGet versions centrally. See [10.0/PACKAGE-VERSIONS.md](../10.0/PACKAGE-VERSIONS.md) for details.

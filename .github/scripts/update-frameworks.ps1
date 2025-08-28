<#+
.SYNOPSIS
	Generic script to duplicate a versioned samples folder (e.g. 9.0 -> 10.0) and update .NET target frameworks & platform versions.

.PARAMETER SourceVersion
	Existing version folder name (e.g. 9.0)

.PARAMETER TargetVersion
	New version folder name to create (e.g. 10.0)

.PARAMETER InPlace
	If set, do not copy; operate directly inside existing TargetVersion (used when the agent already created it).

.PARAMETER ApplyPlatformHeuristic
	Apply heuristic bump for SupportedOSPlatformVersion values when docs not yet consulted.

.EXAMPLE
	pwsh ./update-frameworks.ps1 -SourceVersion 9.0 -TargetVersion 10.0 -ApplyPlatformHeuristic

.NOTES
	This script is intentionally idempotent; running multiple times should not create duplicate changes.
#>

param(
	[Parameter(Mandatory=$true)][string]$SourceVersion,
	[Parameter(Mandatory=$true)][string]$TargetVersion,
	[switch]$InPlace,
	[switch]$ApplyPlatformHeuristic
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Info($msg){ Write-Host "[upgrade] $msg" -ForegroundColor Cyan }
function Write-Warn($msg){ Write-Warning "[upgrade] $msg" }

$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path))
Push-Location $repoRoot

if (-not (Test-Path $SourceVersion)) {
	throw "Source version folder '$SourceVersion' not found at $repoRoot"
}

$sourcePath = Join-Path $repoRoot $SourceVersion
$targetPath = Join-Path $repoRoot $TargetVersion
if (-not $InPlace) {
	if (Test-Path $targetPath) {
		Write-Info "Target folder already exists; skipping copy. Use -InPlace if intentional."
	} else {
		Write-Info "Copying $SourceVersion -> $TargetVersion"
		try {
			Copy-Item -Recurse -Force -Path $sourcePath -Destination $targetPath -ErrorAction Stop
		} catch {
			Write-Warn "Fast copy failed: $($_.Exception.Message)"
			Write-Info "Attempting resilient copy (per-file with retries/skips)..."
			# Ensure target exists
			if (-not (Test-Path $targetPath)) { New-Item -ItemType Directory -Path $targetPath -Force | Out-Null }

			# Enumerate files and copy with best-effort handling
			$allFiles = Get-ChildItem -Path $sourcePath -Recurse -File -Force
			$total = $allFiles.Count
			$copied = 0
			foreach ($file in $allFiles) {
				$relative = $file.FullName.Substring($sourcePath.Length).TrimStart('\\','/')
				$dest = Join-Path $targetPath $relative
				$destDir = Split-Path -Parent $dest
				try {
					if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }
					Copy-Item -LiteralPath $file.FullName -Destination $dest -Force -ErrorAction Stop
					$copied++
					if ($copied -eq 1 -or ($copied % 500 -eq 0) -or $copied -eq $total) {
						Write-Host ("Copied {0} of {1} files" -f $copied, $total)
					}
				} catch {
					Write-Warn ("Skipped file: {0} ({1})" -f $relative, $_.Exception.Message)
				}
			}

			# Ensure empty directories are created
			$allDirs = Get-ChildItem -Path $sourcePath -Recurse -Directory -Force
			foreach ($dir in $allDirs) {
				$relativeDir = $dir.FullName.Substring($sourcePath.Length).TrimStart('\\','/')
				$destDirPath = Join-Path $targetPath $relativeDir
				try { if (-not (Test-Path $destDirPath)) { New-Item -ItemType Directory -Path $destDirPath -Force | Out-Null } } catch {}
			}

			Write-Info ("Resilient copy completed: {0}/{1} files" -f $copied, $total)
		}
	}
} else {
	if (-not (Test-Path $targetPath)) { throw "-InPlace specified but target folder '$TargetVersion' does not exist." }
}

$sourceMajor = $SourceVersion.Split('.')[0]
$targetMajor = $TargetVersion.Split('.')[0]
if (-not ($sourceMajor -match '^[0-9]+$' -and $targetMajor -match '^[0-9]+$')) { throw 'Version folders must start with a numeric major (e.g. 9.0, 10.0)' }

$csprojFiles = Get-ChildItem -Path $targetPath -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }
if (-not $csprojFiles) { Write-Warn "No project files found under $targetPath"; return }

Write-Info "Updating $($csprojFiles.Count) project file(s) from net$sourceMajor.0 to net$targetMajor.0"

foreach ($proj in $csprojFiles) {
	$content = Get-Content -Raw -Path $proj.FullName
	$original = $content

	# Update TargetFrameworks occurrences net{sourceMajor}.0-<rid>
	$content = $content -replace "net$sourceMajor\.0-android","net$targetMajor.0-android" `
							   -replace "net$sourceMajor\.0-ios","net$targetMajor.0-ios" `
							   -replace "net$sourceMajor\.0-maccatalyst","net$targetMajor.0-maccatalyst" `
							   -replace "net$sourceMajor\.0-windows10\.0\.19041\.0","net$targetMajor.0-windows10.0.19041.0" `
							   -replace "net$sourceMajor\.0-tizen","net$targetMajor.0-tizen"

	# Also update singular TargetFramework (rare)
	$content = $content -replace "<TargetFramework>net$sourceMajor\.0","<TargetFramework>net$targetMajor.0"

	# Platform heuristic updates
	if ($ApplyPlatformHeuristic) {
		# Simple regex to bump iOS/MacCatalyst minor versions by +1 major if they match a number like 15.0
		$content = $content -replace "(?<=SupportedOSPlatformVersion[^>]*ios'>)15\.0(?=</SupportedOSPlatformVersion>)","16.0"
		$content = $content -replace "(?<=SupportedOSPlatformVersion[^>]*maccatalyst'>)15\.0(?=</SupportedOSPlatformVersion>)","16.0"
		# Android: ensure at least 24.0
		$content = $content -replace "(?<=SupportedOSPlatformVersion[^>]*android'>)2[0-3]\.0(?=</SupportedOSPlatformVersion>)","24.0"
		# Windows: leave as-is unless lower than 10.0.17763.0
		$content = $content -replace "(?<=SupportedOSPlatformVersion[^>]*windows'>)10\.0\.1(6|5)[0-9]{3}\.0(?=</SupportedOSPlatformVersion>)","10.0.17763.0"
	}

	if ($content -ne $original) {
		Set-Content -Path $proj.FullName -Value $content -Encoding UTF8
		Write-Info "Updated $($proj.FullName.Substring($repoRoot.Length+1))"
	}
}

Write-Info "Update complete."
Pop-Location
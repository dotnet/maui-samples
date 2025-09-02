<#+
.SYNOPSIS
  Preflight validation for an upcoming MAUI samples upgrade to a target major version.

.DESCRIPTION
  Detects whether the target .NET SDK (major) appears available. If not, emits a SIMULATION MODE marker
  so downstream steps & agents know to avoid attempting to fix unsupported TFM errors (e.g. NETSDK1139).

.PARAMETER TargetVersion
  Target version folder name (e.g. 10.0). Only the major component is evaluated for SDK presence.

.PARAMETER OutputJson
  Optional path to write machine-readable JSON summary (contains fields: targetMajor, sdkDetected, simulationMode, dotnetVersion).

.OUTPUTS
  Human-readable status to stdout and optional JSON file.

.NOTES
  Heuristic: we parse `dotnet --version` and installed SDK list; if no SDK whose major equals targetMajor exists, we set simulationMode=true.
  This script performs no source modifications.
#>
param(
  [Parameter(Mandatory=$true)][string]$TargetVersion,
  [string]$OutputJson
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Info($m){ Write-Host "[preflight] $m" -ForegroundColor Cyan }
function Write-Warn($m){ Write-Host "[preflight][WARN] $m" -ForegroundColor Yellow }

if ($TargetVersion -notmatch '^(?<maj>\d+)\.\d+') { throw "TargetVersion must look like '<major>.<minor>' e.g. 10.0" }
$targetMajor = [int]$Matches['maj']

Write-Info "Target major version: $targetMajor"

$dotnetVersion = (dotnet --version) 2>$null
if (-not $dotnetVersion) { Write-Warn "Unable to get current dotnet version; continuing" }

$installed = (dotnet --list-sdks) 2>$null
$sdkDetected = $false
if ($installed) {
  foreach ($line in $installed) {
    if ($line -match '^(?<ver>\d+)\.') {
      $maj = [int]$Matches['ver']
      if ($maj -eq $targetMajor) { $sdkDetected = $true; break }
    }
  }
} else {
  Write-Warn "No SDK list output available. Assuming not detected."
}

if ($sdkDetected) {
  Write-Info "SDK for major $targetMajor detected. Proceed in NORMAL mode."
  $simulationMode = $false
} else {
  Write-Warn "SDK for major $targetMajor not detected. Entering SIMULATION MODE (unsupported TFM errors expected)."
  $simulationMode = $true
}

$result = [pscustomobject]@{
  targetMajor = $targetMajor
  sdkDetected = $sdkDetected
  simulationMode = $simulationMode
  dotnetVersion = $dotnetVersion
  timestamp = (Get-Date -Format o)
}

if ($OutputJson) {
  $dir = Split-Path -Parent $OutputJson
  if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
  $result | ConvertTo-Json -Depth 5 | Set-Content -Path $OutputJson -Encoding UTF8
  Write-Info "Summary JSON written: $OutputJson"
}

# Emit a one-line marker for easy grep in logs/PR body
if ($simulationMode) { Write-Host "SIMULATION_MODE=1" } else { Write-Host "SIMULATION_MODE=0" }

return $result

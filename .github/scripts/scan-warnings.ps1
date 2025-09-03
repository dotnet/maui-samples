<#+
.SYNOPSIS
  Scans build log(s) for warning & error codes, returning a concise JSON summary.

.DESCRIPTION
  Parses msbuild output lines for (warning|error) CODE patterns. In simulation mode (when a sibling
  preflight JSON indicates sdkDetected=false) certain future-TFM NETSDK errors (e.g. NETSDK1139) are
  downgraded to informational classification so they do not pollute prioritization.

.PARAMETER LogPath
  Single log file path.

.PARAMETER OutFile
  Optional path to write JSON summary (UTF-8). If omitted, JSON is written to stdout.

.PARAMETER Quiet
  Suppress console noise except errors.

.EXAMPLE
  pwsh ./.github/scripts/scan-warnings.ps1 -LogPath upgrade-logs/MySample.pre.log -OutFile upgrade-logs/MySample.pre.summary.json
#>
param(
  [Parameter(Mandatory=$true)][string]$LogPath,
  [string]$OutFile,
  [switch]$Quiet
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
if (-not (Test-Path $LogPath)) { throw "Log file not found: $LogPath" }

$lines = Get-Content -Path $LogPath

# Regex captures typical msbuild lines: <path>(line,col): warning CS0618: message OR warning CSxxxx:
$pattern = '(?im)^(?:.*?:\s*)?(warning|error)\s+([A-Z]{2,}[0-9]{4,})'
$items = foreach ($l in $lines) {
  if ($l -match $pattern) {
    [pscustomobject]@{ severity=($matches[1].ToUpper()); code=$matches[2] }
  }
}

if (-not $items) { $items = @() }

# Determine simulation mode (search up two directories for preflight.json)
$simulationMode = $false
$preflight = Get-ChildItem -Path (Join-Path (Split-Path -Parent $LogPath) 'preflight*.json') -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $preflight) {
  # try repo root
  $repoPreflight = Join-Path (Get-Location) 'preflight.json'
  if (Test-Path $repoPreflight) { $preflight = Get-Item $repoPreflight }
}
if ($preflight) {
  try {
    $pf = Get-Content $preflight.FullName -Raw | ConvertFrom-Json
    if ($pf.simulationMode -eq $true) { $simulationMode = $true }
  } catch {}
}

$grouped = $items | Group-Object code | Sort-Object Name | ForEach-Object {
  $sev = ($_.Group.severity | Sort-Object -Unique)
  $hasError = ($sev -contains 'ERROR')
  $codeName = $_.Name
  $simNote = $null
  if ($simulationMode -and $codeName -like 'NETSDK*') {
    # Downgrade unsupported TFM style build errors to note
    $simNote = 'simulation-downgraded'
    $hasError = $false
  }
  [pscustomobject]@{ code=$codeName; count=$_.Count; severities=($sev -join ','); hasError=$hasError; sim=$simNote }
}

$obsolete = $grouped | Where-Object { $_.code -match '^CS06(12|18|72)$' }
$errors = $grouped | Where-Object { $_.hasError }

$summary = [pscustomobject]@{
  log = $LogPath
  totalIssues = $grouped.Count
  errorCount = ($errors | Measure-Object).Count
  obsoleteCount = ($obsolete | Measure-Object).Count
  simulationMode = $simulationMode
  issues = $grouped
  nextAction = if (($errors | Measure-Object).Count -gt 0) { 'Fix build errors first' } elseif (($obsolete | Measure-Object).Count -gt 0) { 'Resolve obsolete APIs (CS0612/CS0618/CS0672) via docs' } elseif ($grouped.Count -gt 0) { 'Review remaining warnings; decide if ignorable' } else { 'Clean - ready to commit' }
}

if ($OutFile) {
  $dir = Split-Path -Parent $OutFile
  if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
  $summary | ConvertTo-Json -Depth 8 | Set-Content -Path $OutFile -Encoding UTF8
  if (-not $Quiet) { Write-Host "[scan-warnings] Summary written: $OutFile" -ForegroundColor Cyan }
} else {
  $summary | ConvertTo-Json -Depth 8
}

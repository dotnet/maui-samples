<#+
.SYNOPSIS
  Builds a single sample solution and captures pre/post fix logs.

.DESCRIPTION
  Wrapper around 'dotnet build' capturing deterministic log + binlog outputs. Designed for
  automation; avoids treating warnings as errors during upgrade discovery phase.

.PARAMETER SolutionPath
  Path to the .sln file.

.PARAMETER LogDir
  Directory for logs. Created if missing.

.PARAMETER Phase
  Either 'pre' or 'post' indicating before or after fixes.

.PARAMETER TargetVersionMajor
  Optional integer used only for traceability (passed as a property; projects may ignore it).

.NOTES
  This script performs no mutation of sources; it must never be edited to add fix logic.
#>
param(
  [Parameter(Mandatory=$true)][string]$SolutionPath,
  [Parameter(Mandatory=$true)][string]$LogDir,
  [ValidateSet('pre','post')][string]$Phase = 'pre',
  [int]$TargetVersionMajor
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Continue'

if (-not (Test-Path $SolutionPath)) { throw "Solution not found: $SolutionPath" }
$solutionName = [IO.Path]::GetFileNameWithoutExtension($SolutionPath)
if (-not (Test-Path $LogDir)) { New-Item -ItemType Directory -Path $LogDir | Out-Null }
$logFile = Join-Path $LogDir "$solutionName.$Phase.log"
$binlog = "$logFile.binlog"

Write-Host "[build-sample] Building $solutionName ($Phase)" -ForegroundColor Cyan

# Use warn-as-error disabled first to collect all warnings. Always show verbosity minimal by default.
$cmd = @('dotnet','build',"$SolutionPath","/bl:$binlog","/nologo","/p:TreatWarningsAsErrors=false")
if ($TargetVersionMajor) {
  $cmd += @('/p:TargetVersionMajor=' + $TargetVersionMajor)
}
Write-Host "[build-sample] cmd: $($cmd -join ' ')" -ForegroundColor DarkGray

& $cmd[0] $cmd[1..($cmd.Length-1)] 2>&1 | Tee-Object -FilePath $logFile

Write-Host "[build-sample] Log: $logFile" -ForegroundColor Cyan

<#+
.SYNOPSIS
	Automated script to upgrade .NET MAUI samples and guide AI to check for deprecations/obsolete APIs.

.PARAMETER SourceVersion
	Existing version folder name (e.g. 9.0)

.PARAMETER TargetVersion
	New version folder name to create (e.g. 10.0)

.PARAMETER InPlace
	If set, do not copy; operate directly inside existing TargetVersion (used when the agent already created it).

.EXAMPLE
	pwsh ./automated-sample-upgrade.ps1 -SourceVersion 9.0 -TargetVersion 10.0

.NOTES
	This script performs framework upgrades and then instructs AI to check documentation for deprecations.
	It is designed to be the single entry point for automated sample upgrades.
#>

param(
	[Parameter(Mandatory=$true)][string]$SourceVersion,
	[Parameter(Mandatory=$true)][string]$TargetVersion,
	[switch]$InPlace,
	[int]$CopyProgressInterval = 200
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Info($msg){ Write-Host "[UPGRADE] $msg" -ForegroundColor Cyan }
function Write-Success($msg){ Write-Host "[SUCCESS] $msg" -ForegroundColor Green }
function Write-Instruction($msg){ Write-Host "[AI INSTRUCTION] $msg" -ForegroundColor Yellow }

$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path))
$scriptsDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Info "Starting automated .NET MAUI sample upgrade from $SourceVersion to $TargetVersion"
Write-Info "Repository root: $repoRoot"

# Step 1: Copy source folder to target folder (if needed)
Write-Info "Step 1: Copying samples folder..."
$sourcePath = Join-Path $repoRoot $SourceVersion
$targetPath = Join-Path $repoRoot $TargetVersion

if (-not (Test-Path $sourcePath)) {
	throw "Source version folder '$SourceVersion' not found at $repoRoot"
}

if (-not $InPlace) {
	if (Test-Path $targetPath) {
		Write-Info "Target folder '$TargetVersion' already exists; skipping copy. Use -InPlace if intentional."
	} else {
		Write-Info "Copying $SourceVersion -> $TargetVersion"
		# Create target root
		New-Item -ItemType Directory -Path $targetPath -Force | Out-Null

		# Enumerate files once to get a stable count and then copy with heartbeat output
		Write-Info "Enumerating files to copy..."
		$allFiles = Get-ChildItem -Path $sourcePath -Recurse -File -Force
		$total = $allFiles.Count
		Write-Info "Found $total files to copy"

		$copied = 0
		foreach ($file in $allFiles) {
			$relative = $file.FullName.Substring($sourcePath.Length).TrimStart('\','/')
			$dest = Join-Path $targetPath $relative
			$destDir = Split-Path -Parent $dest
			if (-not (Test-Path $destDir)) {
				New-Item -ItemType Directory -Path $destDir -Force | Out-Null
			}
			Copy-Item -LiteralPath $file.FullName -Destination $dest -Force
			$copied++
			if ($copied -eq 1 -or ($copied % [Math]::Max(1,$CopyProgressInterval) -eq 0) -or $copied -eq $total) {
				Write-Host ("Copied {0} of {1} files" -f $copied, $total)
			}
		}

		# Ensure empty directories (like Screenshots/) are created as well
		$allDirs = Get-ChildItem -Path $sourcePath -Recurse -Directory -Force
		foreach ($dir in $allDirs) {
			$relativeDir = $dir.FullName.Substring($sourcePath.Length).TrimStart('\','/')
			$destDirPath = Join-Path $targetPath $relativeDir
			if (-not (Test-Path $destDirPath)) {
				New-Item -ItemType Directory -Path $destDirPath -Force | Out-Null
			}
		}

		Write-Success "Folder copied successfully"
	}
} else {
	if (-not (Test-Path $targetPath)) { 
		throw "-InPlace specified but target folder '$TargetVersion' does not exist." 
	}
	Write-Info "Using existing target folder (InPlace mode)"
}

# Step 2: Run the existing framework update script
Write-Info "Step 2: Updating frameworks and platform versions..."
$updateFrameworksScript = Join-Path $scriptsDir "update-frameworks.ps1"

if (-not (Test-Path $updateFrameworksScript)) {
	throw "Framework update script not found at: $updateFrameworksScript"
}

$params = @{
	SourceVersion = $SourceVersion
	TargetVersion = $TargetVersion
	ApplyPlatformHeuristic = $true
}

if ($InPlace) {
	$params.InPlace = $true
}

try {
	& $updateFrameworksScript @params
	Write-Success "Framework updates completed successfully"
} catch {
	throw "Framework update failed: $_"
}

# Step 3: Run preflight check
Write-Info "Step 3: Running preflight SDK validation..."
$preflightScript = Join-Path $scriptsDir "preflight-upgrade.ps1"
$preflightJson = Join-Path $repoRoot "preflight.json"

if (Test-Path $preflightScript) {
	try {
		$preflightResult = & $preflightScript -TargetVersion $TargetVersion -OutputJson $preflightJson
		if ($preflightResult.simulationMode) {
			Write-Info "SIMULATION MODE detected - SDK for .NET $($preflightResult.targetMajor) not available"
		} else {
			Write-Success "Normal mode - SDK for .NET $($preflightResult.targetMajor) detected"
		}
	} catch {
		Write-Warning "Preflight check failed: $_"
	}
} else {
	Write-Warning "Preflight script not found, skipping SDK validation"
}

# Step 4: Generate sample analysis report and build samples
Write-Info "Step 4: Analyzing samples and running builds..."
$targetPath = Join-Path $repoRoot $TargetVersion
$logDir = Join-Path $targetPath "upgrade-logs"
if (-not (Test-Path $logDir)) {
	New-Item -ItemType Directory -Path $logDir -Force | Out-Null
}

# Find all solutions instead of just projects for building
$allSolutions = Get-ChildItem -Path $targetPath -Recurse -Filter "*.sln" | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }
$allSamples = Get-ChildItem -Path $targetPath -Recurse -Filter "*.csproj" | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }

Write-Info "Found $($allSolutions.Count) solutions and $($allSamples.Count) projects"

$sampleReport = @()
$buildScript = Join-Path $scriptsDir "build-sample.ps1"
$scanScript = Join-Path $scriptsDir "scan-warnings.ps1"

# Get target major version for build script
$targetMajor = if ($TargetVersion -match '^(\d+)\.') { [int]$Matches[1] } else { $null }

# Build each solution and scan for warnings
foreach ($solution in $allSolutions | Sort-Object Name) {
	$solutionName = [System.IO.Path]::GetFileNameWithoutExtension($solution.Name)
	$relativePath = $solution.FullName.Substring($repoRoot.Length + 1)
	
	Write-Info "Building and scanning: $solutionName"
	
	$buildSuccess = $false
	$scanSuccess = $false
	
	# Build the solution
	if (Test-Path $buildScript) {
		try {
			$buildParams = @{
				SolutionPath = $solution.FullName
				LogDir = $logDir
				Phase = 'pre'
			}
			if ($targetMajor) {
				$buildParams.TargetVersionMajor = $targetMajor
			}
			
			& $buildScript @buildParams
			$buildSuccess = $true
			Write-Success "Build completed for $solutionName"
		} catch {
			Write-Warning "Build failed for $solutionName`: $_"
		}
	}
	
	# Scan for warnings if build completed
	$logFile = Join-Path $logDir "$solutionName.pre.log"
	if ($buildSuccess -and (Test-Path $logFile) -and (Test-Path $scanScript)) {
		try {
			$summaryFile = Join-Path $logDir "$solutionName.pre.summary.json"
			& $scanScript -LogPath $logFile -OutFile $summaryFile -Quiet
			$scanSuccess = $true
			
			# Read the summary for reporting
			if (Test-Path $summaryFile) {
				$summary = Get-Content $summaryFile -Raw | ConvertFrom-Json
				Write-Info "  Errors: $($summary.errorCount), Obsolete: $($summary.obsoleteCount), Total Issues: $($summary.totalIssues)"
				Write-Info "  Next Action: $($summary.nextAction)"
			}
		} catch {
			Write-Warning "Warning scan failed for $solutionName`: $_"
		}
	}
	
	$sampleReport += [PSCustomObject]@{
		Name = $solutionName
		ProjectPath = $relativePath
		Directory = $solution.Directory.FullName.Substring($repoRoot.Length + 1)
		BuildSuccess = $buildSuccess
		ScanSuccess = $scanSuccess
		LogFile = if ($buildSuccess) { $logFile } else { $null }
		SummaryFile = if ($scanSuccess) { Join-Path $logDir "$solutionName.pre.summary.json" } else { $null }
	}
}

Write-Success "Completed analysis of $($sampleReport.Count) samples"
Write-Info "Build logs and summaries available in: $($logDir.Substring($repoRoot.Length + 1))"

# Step 5: Output AI instructions
Write-Info "Step 5: Generating AI instructions for deprecation analysis..."

Write-Host ""
Write-Host "================================================================================================" -ForegroundColor Magenta
Write-Host "AI AUTOMATION INSTRUCTIONS" -ForegroundColor Magenta
Write-Host "================================================================================================" -ForegroundColor Magenta
Write-Host ""

Write-Instruction "TASK: Check .NET MAUI documentation for deprecated/obsolete APIs and update samples accordingly"
Write-Instruction ""
Write-Instruction "STEP 1: Use the Microsoft Documentation MCP to read the latest .NET MAUI what's new documentation:"
Write-Instruction "        https://learn.microsoft.com/en-us/dotnet/maui/whats-new/"
Write-Instruction ""
Write-Instruction "STEP 2: Review the build and scan results below before proceeding with sample analysis"
Write-Instruction ""

# Display build and scan results
$successfulBuilds = ($sampleReport | Where-Object BuildSuccess).Count
$successfulScans = ($sampleReport | Where-Object ScanSuccess).Count
$failedBuilds = ($sampleReport | Where-Object { -not $_.BuildSuccess }).Count

Write-Host "BUILD SUMMARY:" -ForegroundColor White
Write-Host "  Successful builds: $successfulBuilds" -ForegroundColor Green
Write-Host "  Failed builds: $failedBuilds" -ForegroundColor $(if ($failedBuilds -gt 0) { 'Red' } else { 'Gray' })
Write-Host "  Successful scans: $successfulScans" -ForegroundColor Green
Write-Host ""

if ($failedBuilds -gt 0) {
	Write-Host "FAILED BUILDS:" -ForegroundColor Red
	foreach ($failed in ($sampleReport | Where-Object { -not $_.BuildSuccess })) {
		Write-Host "  • $($failed.Name)" -ForegroundColor Red
		Write-Host "    Path: $($failed.Directory)" -ForegroundColor Gray
	}
	Write-Host ""
}

# Show samples with scan results and priorities
$samplesWithIssues = @()
foreach ($sample in ($sampleReport | Where-Object ScanSuccess)) {
	if (Test-Path $sample.SummaryFile) {
		try {
			$summary = Get-Content $sample.SummaryFile -Raw | ConvertFrom-Json
			$samplesWithIssues += [PSCustomObject]@{
				Name = $sample.Name
				ErrorCount = $summary.errorCount
				ObsoleteCount = $summary.obsoleteCount
				TotalIssues = $summary.totalIssues
				NextAction = $summary.nextAction
				Directory = $sample.Directory
				SummaryFile = $sample.SummaryFile
			}
		} catch {
			Write-Warning "Could not read summary for $($sample.Name)"
		}
	}
}

if ($samplesWithIssues.Count -gt 0) {
	Write-Host "PRIORITIZED SAMPLES (by issues):" -ForegroundColor White
	$prioritized = $samplesWithIssues | Sort-Object ErrorCount -Descending | Sort-Object ObsoleteCount -Descending | Sort-Object TotalIssues -Descending
	foreach ($sample in $prioritized) {
		$color = if ($sample.ErrorCount -gt 0) { 'Red' } elseif ($sample.ObsoleteCount -gt 0) { 'Yellow' } elseif ($sample.TotalIssues -gt 0) { 'Cyan' } else { 'Green' }
		Write-Host "  • $($sample.Name)" -ForegroundColor $color
		Write-Host "    Errors: $($sample.ErrorCount), Obsolete: $($sample.ObsoleteCount), Total: $($sample.TotalIssues)" -ForegroundColor Gray
		Write-Host "    Action: $($sample.NextAction)" -ForegroundColor Gray
		Write-Host "    Summary: $($sample.SummaryFile.Substring($repoRoot.Length + 1))" -ForegroundColor Gray
		Write-Host ""
	}
}

Write-Instruction "STEP 3: For samples with build errors (red), fix compilation issues first"
Write-Instruction "STEP 4: For samples with obsolete warnings (yellow), use Microsoft documentation to resolve CS0612/CS0618/CS0672 warnings"
Write-Instruction "STEP 5: For samples with other issues (cyan), review and determine if warnings can be safely ignored"
Write-Instruction ""
Write-Instruction "SAMPLES TO ANALYZE:"

foreach ($sample in $sampleReport | Sort-Object Name) {
	Write-Host "  • $($sample.Name)" -ForegroundColor White
	Write-Host "    Path: $($sample.Directory)" -ForegroundColor Gray
	Write-Host "    Project: $($sample.ProjectPath)" -ForegroundColor Gray
	Write-Host ""
}

Write-Instruction "CONSTRAINTS AND WORKFLOW:"
Write-Instruction "- START with samples that have build errors (they block everything else)"
Write-Instruction "- FOCUS on CS0612/CS0618/CS0672 obsolete warnings - use Microsoft documentation to find replacements"  
Write-Instruction "- DO NOT make style changes, refactoring, or other improvements"
Write-Instruction "- DO NOT update package versions unless specifically required for deprecation fixes"
Write-Instruction "- DO NOT modify project structure or add new features"
Write-Instruction "- DO NOT modify anything in the source folder ($SourceVersion) - only work in the target folder ($TargetVersion)"
Write-Instruction "- USE the generated summary files to understand specific issues in each sample"
Write-Instruction "- REBUILD samples after fixes using: pwsh ./.github/scripts/build-sample.ps1 -SolutionPath <path> -LogDir $($TargetVersion)/upgrade-logs -Phase post"
Write-Instruction "- RESCAN after rebuilding using: pwsh ./.github/scripts/scan-warnings.ps1 -LogPath <post.log> -OutFile <post.summary.json> -Quiet"

Write-Host ""
Write-Host "================================================================================================" -ForegroundColor Magenta
Write-Host "END OF AI INSTRUCTIONS" -ForegroundColor Magenta
Write-Host "================================================================================================" -ForegroundColor Magenta
Write-Host ""

Write-Success "Automated upgrade process completed!"
Write-Success "Preflight check: $(if (Test-Path $preflightJson) { 'Completed' } else { 'Skipped' })"
Write-Success "Framework updates: Completed"
Write-Success "Sample builds: $successfulBuilds/$($sampleReport.Count) successful"
Write-Success "Warning scans: $successfulScans/$($sampleReport.Count) successful"
Write-Success "Build logs and summaries: $($logDir.Substring($repoRoot.Length + 1))"
Write-Success "The AI should now proceed with issue resolution using the prioritized list above."

param(
    [Parameter(Mandatory)]
    [AllowEmptyCollection()]
    [string[]] $ChangedFiles,
    [string] $ExcludedProjectsFile
)

$ErrorActionPreference = 'Stop'
$root = (Get-Location).Path
$projects = foreach ($file in $ChangedFiles) {
    # Documentation changes never require a project build.
    if ($file -like '*.md') {
        continue
    }

    $path = [System.IO.Path]::GetFullPath($file, $root)
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        continue
    }

    if ($file -like '*.csproj') {
        $path
        continue
    }

    # Build the nearest project at or above the changed file.
    $folder = Split-Path -Parent $path
    while ($folder -and ($folder -eq $root -or $folder.StartsWith($root + [System.IO.Path]::DirectorySeparatorChar))) {
        $found = @(Get-ChildItem -LiteralPath $folder -Filter '*.csproj' -File)
        if ($found.Count -gt 0) {
            $found[0].FullName
            break
        }
        if ($folder -eq $root) {
            break
        }
        $folder = Split-Path -Parent $folder
    }
}

$excluded = @()
if ($ExcludedProjectsFile -and (Test-Path -LiteralPath $ExcludedProjectsFile)) {
    $excluded = @(Get-Content -LiteralPath $ExcludedProjectsFile | Where-Object { $_ -notmatch '^\s*#' -and $_ -match '\S' })
}

$projects | ForEach-Object {
    './' + [System.IO.Path]::GetRelativePath($root, $_).Replace('\', '/')
} | Sort-Object -Unique | Where-Object { $_ -notin $excluded }

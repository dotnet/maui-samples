param(
    [Parameter(Mandatory)]
    [AllowEmptyCollection()]
    [string[]] $ChangedFiles,
    [string] $ExcludedProjectsFile
)

$ErrorActionPreference = 'Stop'
$root = (Get-Location).Path
$projects = foreach ($file in $ChangedFiles) {
    if ($file -like '*.md') {
        continue
    }

    $path = [System.IO.Path]::GetFullPath($file, $root)
    if ($file -like '*.csproj') {
        if (Test-Path -LiteralPath $path -PathType Leaf) {
            $path
        }
        continue
    }

    $folder = Split-Path -Parent $path
    if ((Split-Path -Leaf $file) -in @('Directory.Build.props', 'Directory.Build.targets', 'Directory.Packages.props', 'global.json', 'NuGet.config')) {
        if (Test-Path -LiteralPath $folder -PathType Container) {
            Get-ChildItem -LiteralPath $folder -Filter '*.csproj' -File -Recurse | ForEach-Object FullName
        }
        continue
    }

    # Deleted source files still affect any remaining project in their parent folders.
    while ($folder -and ($folder -eq $root -or $folder.StartsWith($root + [System.IO.Path]::DirectorySeparatorChar))) {
        if (Test-Path -LiteralPath $folder -PathType Container) {
            $found = @(Get-ChildItem -LiteralPath $folder -Filter '*.csproj' -File)
            if ($found.Count -gt 0) {
                $found | ForEach-Object FullName
                break
            }
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

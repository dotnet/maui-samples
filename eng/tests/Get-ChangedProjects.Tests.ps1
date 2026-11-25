$ErrorActionPreference = 'Stop'
$script = Join-Path $PSScriptRoot '../Get-ChangedProjects.ps1'
$fixture = Join-Path ([System.IO.Path]::GetTempPath()) "maui-changed-projects-$([guid]::NewGuid())"
New-Item -ItemType Directory -Path "$fixture/11.0/App/Views", "$fixture/11.0/Other", "$fixture/.github/workflows" -Force | Out-Null
foreach ($file in @('11.0/App/App.csproj', '11.0/App/Views/Page.xaml', '11.0/App/README.md', '11.0/Other/Other.csproj', '11.0/Other/Other.cs', '11.0/Directory.Build.props', 'README.md', '.github/workflows/build-pr.yml')) {
    New-Item -ItemType File -Path "$fixture/$file" | Out-Null
}
'./11.0/Other/Other.csproj' | Set-Content "$fixture/excluded.txt"

function Assert-Projects([string] $Name, [string[]] $Files, [string[]] $Expected, [string] $Exclusions) {
    $actual = @(& $script -ChangedFiles $Files -ExcludedProjectsFile $Exclusions)
    if (($actual -join "`n") -cne (($Expected | Sort-Object) -join "`n")) {
        throw "${Name}: expected [$($Expected -join ', ')], got [$($actual -join ', ')]."
    }
    Write-Output "PASS: $Name"
}

Push-Location $fixture
try {
    Assert-Projects 'Documentation and workflow-only changes' @('README.md', '11.0/App/README.md', '.github/workflows/build-pr.yml') @()
    Assert-Projects 'Empty diff' @() @()
    Assert-Projects 'Source changes build the nearest project once' @('11.0/App/Views/Page.xaml', '11.0/App/App.csproj') @('./11.0/App/App.csproj')
    Assert-Projects 'Platform exclusions' @('11.0/Other/Other.cs', '11.0/App/Views/Page.xaml') @('./11.0/App/App.csproj') './excluded.txt'
    Assert-Projects 'Files without a parent project are ignored' @('11.0/Directory.Build.props') @()
    Assert-Projects 'Deleted files are ignored' @('11.0/App/Removed.cs', '11.0/App/Removed.csproj') @()
}
finally {
    Pop-Location
    Remove-Item -LiteralPath $fixture -Recurse -Force
}

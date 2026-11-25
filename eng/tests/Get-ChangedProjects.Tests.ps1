$ErrorActionPreference = 'Stop'
$script = Join-Path $PSScriptRoot '../Get-ChangedProjects.ps1'
$fixture = Join-Path ([System.IO.Path]::GetTempPath()) "maui-changed-projects-$([guid]::NewGuid())"
New-Item -ItemType Directory -Path "$fixture/11.0/App/Views", "$fixture/11.0/Other", "$fixture/10.0/App", "$fixture/.github/workflows" -Force | Out-Null
foreach ($file in @('11.0/App/App.csproj', '11.0/App/Library.csproj', '11.0/App/Views/Page.xaml', '11.0/Other/Other.csproj', '10.0/App/App.csproj', '11.0/Directory.Build.props')) {
    New-Item -ItemType File -Path "$fixture/$file" | Out-Null
}
'./11.0/App/App.csproj', './11.0/App/Library.csproj' | Set-Content "$fixture/excluded.txt"

function Assert-Projects([string] $Name, [string[]] $Files, [string[]] $Expected, [string] $Exclusions) {
    $actual = @(& $script -ChangedFiles $Files -ExcludedProjectsFile $Exclusions)
    if (($actual -join "`n") -cne (($Expected | Sort-Object) -join "`n")) {
        throw "${Name}: expected [$($Expected -join ', ')], got [$($actual -join ', ')]."
    }
    Write-Output "PASS: $Name"
}

Push-Location $fixture
try {
    Assert-Projects 'Documentation and workflow-only changes' @('README.md', '11.0/App/README.md', '.github/workflows/build-pr.yml', 'eng/Get-ChangedProjects.ps1') @()
    Assert-Projects 'Empty diff' @() @()
    Assert-Projects 'Source and resource changes select all sibling projects once' @('11.0/App/Views/Page.xaml', '11.0/App/App.cs') @('./11.0/App/App.csproj', './11.0/App/Library.csproj')
    Assert-Projects 'Direct project edit' @('11.0/App/App.csproj') @('./11.0/App/App.csproj')
    Assert-Projects 'Platform exclusions' @('11.0/App/Views/Page.xaml') @() './excluded.txt'
    Assert-Projects 'Shared build settings select descendant projects only' @('11.0/Directory.Build.props') @('./11.0/App/App.csproj', './11.0/App/Library.csproj', './11.0/Other/Other.csproj')
    Assert-Projects 'Deleted nested source still rebuilds its project' @('11.0/App/Deleted/Old.cs') @('./11.0/App/App.csproj', './11.0/App/Library.csproj')
    Assert-Projects 'Deleted project is not built' @('11.0/App/Removed.csproj') @()
    Assert-Projects 'Renamed file rebuilds both surviving projects' @('11.0/App/Old.cs', '11.0/Other/New.cs') @('./11.0/App/App.csproj', './11.0/App/Library.csproj', './11.0/Other/Other.csproj')
}
finally {
    Pop-Location
    Remove-Item -LiteralPath $fixture -Recurse -Force
}

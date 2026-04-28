#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Scaffolds a local demo site that exercises Umbraco.Commerce.Automate against
    the official Umbraco Commerce demo store.

.DESCRIPTION
    Clones the Umbraco Commerce demo store into ./demo, adds a project reference
    from the demo's web project to the local Umbraco.Commerce.Automate package,
    and generates a git-ignored Umbraco.Commerce.Automate.local.slnx at the repo
    root combining src + tests + the demo projects.

    Both the demo folder and *.local.slnx are git-ignored. Re-run with -Force to
    wipe and re-clone.

.PARAMETER Path
    Target folder for the cloned demo. Defaults to "demo".

.PARAMETER Repo
    Git URL of the demo store. Defaults to the official Umbraco repo.

.PARAMETER Branch
    Branch, tag, or commit to check out. Defaults to "main".

.PARAMETER DemoSubPath
    Path within the cloned repo containing the demo solution. Defaults to the
    repo root.

.PARAMETER WebProject
    Path (relative to DemoSubPath) of the web project that should reference the
    local Automate package. Auto-detected when omitted.

.PARAMETER Force
    Delete an existing demo folder before cloning.

.EXAMPLE
    ./setup-demo.ps1

.EXAMPLE
    ./setup-demo.ps1 -Branch v17/dev -Force
#>
[CmdletBinding()]
param(
    [string]$Path = "demo",
    [string]$Repo = "https://github.com/umbraco/Umbraco.Commerce.DemoStore.git",
    [string]$Branch = "main",
    [string]$DemoSubPath = "",
    [string]$WebProject,
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$targetPath = Join-Path $repoRoot $Path
$localSln = Join-Path $repoRoot "Umbraco.Commerce.Automate.local.slnx"

if (Test-Path $targetPath) {
    if ($Force) {
        Write-Host "Removing existing $targetPath" -ForegroundColor Yellow
        Remove-Item -Recurse -Force $targetPath
        if (Test-Path $localSln) { Remove-Item -Force $localSln }
    }
    else {
        throw "Path '$targetPath' already exists. Re-run with -Force to overwrite."
    }
}

Write-Host "Cloning $Repo ($Branch) -> $targetPath" -ForegroundColor Cyan
git clone --depth 1 --branch $Branch $Repo $targetPath
if ($LASTEXITCODE -ne 0) { throw "git clone failed" }

$demoRoot = if ($DemoSubPath) { Join-Path $targetPath $DemoSubPath } else { $targetPath }
if (-not (Test-Path $demoRoot)) {
    throw "Demo sub-path '$demoRoot' not found. Use -DemoSubPath to point at the demo solution."
}

if (-not $WebProject) {
    $WebProject = Get-ChildItem -Path $demoRoot -Recurse -Filter "*.Web.csproj" |
        Select-Object -First 1 -ExpandProperty FullName
    if (-not $WebProject) {
        throw "Could not auto-detect a *.Web.csproj under $demoRoot. Pass -WebProject explicitly."
    }
}
elseif (-not [System.IO.Path]::IsPathRooted($WebProject)) {
    $WebProject = Join-Path $demoRoot $WebProject
}

if (-not (Test-Path $WebProject)) {
    throw "Web project '$WebProject' not found."
}

$automateProject = Join-Path $repoRoot "src/Umbraco.Commerce.Automate/Umbraco.Commerce.Automate.csproj"
if (-not (Test-Path $automateProject)) {
    throw "Automate project not found at $automateProject"
}

$packagesPropsPath = Join-Path $repoRoot "Directory.Packages.props"
$automateMetaVersion = $null
if (Test-Path $packagesPropsPath) {
    $packagesXml = [xml](Get-Content -Raw -Path $packagesPropsPath)
    $automateMetaVersion = $packagesXml.Project.ItemGroup.PackageVersion |
        Where-Object { $_.Include -eq "Umbraco.Automate" } |
        Select-Object -First 1 -ExpandProperty Version
}
if (-not $automateMetaVersion) {
    throw 'Could not find Umbraco.Automate version in Directory.Packages.props. Add a <PackageVersion Include="Umbraco.Automate" Version="..." />.'
}

Write-Host "Adding Umbraco.Automate $automateMetaVersion package to $(Split-Path -Leaf $WebProject)" -ForegroundColor Cyan
dotnet add $WebProject package Umbraco.Automate --version $automateMetaVersion
if ($LASTEXITCODE -ne 0) { throw "dotnet add package Umbraco.Automate failed" }

Write-Host "Adding Automate reference to $(Split-Path -Leaf $WebProject)" -ForegroundColor Cyan
dotnet add $WebProject reference $automateProject
if ($LASTEXITCODE -ne 0) { throw "dotnet add reference failed" }

$webProjectDir = Split-Path -Parent $WebProject
$appsettingsPath = Join-Path $webProjectDir "appsettings.json"
if (Test-Path $appsettingsPath) {
    Write-Host "Syncing umbracoAutomateDbDSN in appsettings.json" -ForegroundColor Cyan
    $appsettings = Get-Content -Raw -Path $appsettingsPath | ConvertFrom-Json -AsHashtable
    if (-not $appsettings.ContainsKey('ConnectionStrings')) {
        $appsettings['ConnectionStrings'] = [ordered]@{}
    }
    $cs = $appsettings['ConnectionStrings']
    $dbDsn = $cs['umbracoDbDSN']
    $dbProvider = $cs['umbracoDbDSN_ProviderName']
    if ($null -eq $dbDsn) {
        Write-Host "  umbracoDbDSN not found — skipping sync." -ForegroundColor Yellow
    }
    else {
        $cs['umbracoAutomateDbDSN'] = $dbDsn
        if ($null -ne $dbProvider) {
            $cs['umbracoAutomateDbDSN_ProviderName'] = $dbProvider
        }
        $appsettings | ConvertTo-Json -Depth 32 | Set-Content -Path $appsettingsPath -Encoding UTF8
    }
}
else {
    Write-Host "appsettings.json not found alongside web project — skipping DSN sync." -ForegroundColor Yellow
}

function ConvertTo-RelativePath {
    param([string]$From, [string]$To)
    $fromUri = New-Object System.Uri(($From.TrimEnd('\','/') + [System.IO.Path]::DirectorySeparatorChar))
    $toUri = New-Object System.Uri($To)
    $rel = [System.Uri]::UnescapeDataString($fromUri.MakeRelativeUri($toUri).ToString())
    return $rel.Replace('/', '\')
}

$testsProject = Join-Path $repoRoot "tests/Umbraco.Commerce.Automate.Tests.Unit/Umbraco.Commerce.Automate.Tests.Unit.csproj"
$demoProjects = Get-ChildItem -Path $demoRoot -Recurse -Filter "*.csproj" | ForEach-Object { $_.FullName }

$srcEntry   = ConvertTo-RelativePath -From $repoRoot -To $automateProject
$testsEntry = ConvertTo-RelativePath -From $repoRoot -To $testsProject
$demoEntries = $demoProjects | ForEach-Object { ConvertTo-RelativePath -From $repoRoot -To $_ }

Write-Host "Generating $(Split-Path -Leaf $localSln)" -ForegroundColor Cyan
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('<Solution>')
[void]$sb.AppendLine('  <Folder Name="/src/">')
[void]$sb.AppendLine("    <Project Path=`"$srcEntry`" />")
[void]$sb.AppendLine('  </Folder>')
[void]$sb.AppendLine('  <Folder Name="/tests/">')
[void]$sb.AppendLine("    <Project Path=`"$testsEntry`" />")
[void]$sb.AppendLine('  </Folder>')
[void]$sb.AppendLine('  <Folder Name="/demo/">')
foreach ($entry in $demoEntries) {
    [void]$sb.AppendLine("    <Project Path=`"$entry`" />")
}
[void]$sb.AppendLine('  </Folder>')
[void]$sb.AppendLine('</Solution>')
Set-Content -Path $localSln -Value $sb.ToString() -Encoding UTF8

Write-Host ""
Write-Host "Demo ready." -ForegroundColor Green
Write-Host "  Demo solution: $demoRoot"
Write-Host "  Web project:   $WebProject"
Write-Host "  Local slnx:    $localSln"
Write-Host ""
Write-Host "Open $(Split-Path -Leaf $localSln) in your IDE, or run:" -ForegroundColor Cyan
Write-Host "  dotnet run --project `"$WebProject`""

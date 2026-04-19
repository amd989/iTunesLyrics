# ClickOnce build + publish to gh-pages.
# Based on https://janjones.me/posts/clickonce-installer-build-publish-github/.

[CmdletBinding(PositionalBinding=$false)]
param (
    [switch]$OnlyBuild=$false,
    [string]$SignedArtifactDir=""
)

$appName = "iTuneslyrics"
$projDir = "iTuneslyrics"

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

Write-Output "Working directory: $pwd"

# Find MSBuild.
$msBuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
    -prerelease | Select-Object -First 1
Write-Output "MSBuild: $((Get-Command $msBuildPath).Path)"

# Load current Git tag.
$tag = $(git describe --tags)
Write-Output "Tag: $tag"

# Parse tag into a three-number version.
$version = $tag.Split('-')[0].TrimStart('v')
$version = "$version.0"
Write-Output "Version: $version"

# Clean output directory.
$publishDir = "bin/publish"
$outDir = "$projDir/$publishDir"
if (Test-Path $outDir) {
    Remove-Item -Path $outDir -Recurse
}

# Publish the application.
Push-Location $projDir
try {
    Write-Output "Restoring:"
    dotnet restore iTuneslyrics.csproj
    Write-Output "Publishing:"
    $msBuildVerbosityArg = "/v:m"
    if ($env:CI) {
        $msBuildVerbosityArg = ""
    }
    & $msBuildPath iTuneslyrics.csproj /target:publish /p:PublishProfile=ClickOnceProfile `
        /p:ApplicationVersion=$version /p:Version=$version /p:Configuration=Release `
        /p:PublishDir=$publishDir\ `
        $msBuildVerbosityArg
    if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit 1 }

    # Verify ClickOnce artifacts were produced.
    $appFilesDir = "$publishDir/Application Files"
    if (-Not (Test-Path $appFilesDir)) {
        throw "ClickOnce artifacts not found at '$appFilesDir'. Publish likely ran in plain-copy mode — ensure Properties/PublishProfiles/ClickOnceProfile.pubxml is present and PublishProfile resolved correctly."
    }
    $publishSize = (Get-ChildItem -Path $appFilesDir -Recurse |
        Measure-Object -Property Length -Sum).Sum / 1Mb
    Write-Output ("Published size: {0:N2} MB" -f $publishSize)
}
finally {
    Pop-Location
}

if ($OnlyBuild) {
    Write-Output "Build finished."
    exit
}

# Determine which directory to deploy (signed or unsigned).
$deployDir = $outDir
if ($SignedArtifactDir) {
    if (-Not (Test-Path $SignedArtifactDir)) {
        throw "Signed artifact directory not found: $SignedArtifactDir"
    }
    $deployDir = $SignedArtifactDir
    Write-Output "Using signed artifacts from: $SignedArtifactDir"
} else {
    Write-Output "Using unsigned artifacts from: $outDir"
}

# Clone `gh-pages` branch.
$ghPagesDir = "gh-pages"
if (-Not (Test-Path $ghPagesDir)) {
    git clone $(git config --get remote.origin.url) -b gh-pages `
        --depth 1 --single-branch $ghPagesDir
}

Push-Location $ghPagesDir
try {
    # Ensure all files are stored as binary — prevents git from normalizing line endings
    # in ClickOnce manifest XML files, which would break hash verification on install.
    if (-Not (Test-Path ".gitattributes") -or (Get-Content ".gitattributes" -Raw) -notmatch '\* -text') {
        Set-Content ".gitattributes" "* -text"
    }

    Write-Output "Removing previous files..."
    if (Test-Path "Application Files") {
        Remove-Item -Path "Application Files" -Recurse
    }
    if (Test-Path "$appName.application") {
        Remove-Item -Path "$appName.application"
    }

    Write-Output "Copying new files..."
    Copy-Item -Path "../$deployDir/Application Files","../$deployDir/$appName.application" `
        -Destination . -Recurse

    Write-Output "Staging..."
    git add -A
    Write-Output "Committing..."
    git commit -m "Update to v$version"

    git push
} finally {
    Pop-Location
}

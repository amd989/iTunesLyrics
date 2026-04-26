# Build portable ZIP distribution

[CmdletBinding(PositionalBinding=$false)]
param (
    [string]$Version = ""
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

Write-Output "Building portable distribution..."

if ([string]::IsNullOrEmpty($Version)) {
    $tag = $(git describe --tags 2>$null)
    if ($LASTEXITCODE -ne 0) {
        Write-Error "No Git tag found. Please specify -Version or create a Git tag."
        exit 1
    }
    $Version = $tag.TrimStart('v')
    Write-Output "Detected version from Git tag: $Version"
} else {
    Write-Output "Using specified version: $Version"
}

$projDir = "iTuneslyrics"
$outputDir = "bin/portable"
$publishDir = "$projDir/$outputDir"

# Clean output directory
if (Test-Path $publishDir) {
    Remove-Item -Path $publishDir -Recurse -Force
}

# Publish self-contained for portability
Write-Output "Publishing:"
dotnet publish "$projDir/iTuneslyrics.csproj" `
    -c Release `
    -o "$publishDir" `
    -p:Version=$Version `
    --self-contained false
if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit 1 }

# Create ZIP
$zipName = "ituneslyrics-$Version-portable.zip"
Write-Output "Creating ZIP archive: $zipName"

if (Test-Path $zipName) {
    Remove-Item $zipName -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory(
    (Resolve-Path $publishDir).Path,
    (Join-Path (Get-Location) $zipName),
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false
)

$zipSize = (Get-Item $zipName).Length / 1MB
Write-Output ("Created portable ZIP: {0:N2} MB" -f $zipSize)
Write-Output "Location: $(Resolve-Path $zipName)"

$hash = (Get-FileHash -Path $zipName -Algorithm SHA256).Hash.ToLower()
Write-Output ""
Write-Output "SHA256 hash:"
Write-Output $hash

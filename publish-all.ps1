#!/usr/bin/env pwsh
# Multi-platform portable build script for TempConvPro

$ErrorActionPreference = "Stop"

# Define target platforms
$runtimes = @(
    "win-x64",
    "win-arm64",
    "linux-x64",
    "linux-arm64",
    "osx-x64",
    "osx-arm64"
)

# Configuration
$configuration = "Release"
$publishDir = "publish"
$projectFile = "TempConvPro.csproj"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Portable Builds for All Platforms" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous publish directory
if (Test-Path $publishDir) {
    Write-Host "Cleaning previous publish directory..." -ForegroundColor Yellow
    Remove-Item -Path $publishDir -Recurse -Force
}

# Create publish directory
New-Item -ItemType Directory -Path $publishDir -Force | Out-Null

# Build for each platform
foreach ($runtime in $runtimes) {
    Write-Host "Publishing for $runtime..." -ForegroundColor Green

    $outputPath = Join-Path $publishDir $runtime

    try {
        # Restore for the specific runtime first
        dotnet restore $projectFile -r $runtime /p:IsPublishing=true | Out-Null

        # Then publish
        dotnet publish $projectFile `
            -c $configuration `
            -r $runtime `
            -o $outputPath `
            --no-restore `
            --self-contained true `
            /p:PublishSingleFile=true `
            /p:PublishTrimmed=true `
            /p:IsPublishing=true

        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Successfully published for $runtime" -ForegroundColor Green
        } else {
            Write-Host "✗ Failed to publish for $runtime" -ForegroundColor Red
        }
    } catch {
        Write-Host "✗ Error publishing for $runtime : $_" -ForegroundColor Red
    }

    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Cyan
Write-Host "Output directory: $publishDir" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

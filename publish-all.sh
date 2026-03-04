#!/bin/bash
# Multi-platform portable build script for TempConvPro

set -e

# Define target platforms
runtimes=(
    "win-x64"
    "win-arm64"
    "linux-x64"
    "linux-arm64"
    "osx-x64"
    "osx-arm64"
)

# Configuration
configuration="Release"
publishDir="publish"
projectFile="TempConvPro.csproj"

echo "========================================"
echo "Building Portable Builds for All Platforms"
echo "========================================"
echo ""

# Clean previous publish directory
if [ -d "$publishDir" ]; then
    echo "Cleaning previous publish directory..."
    rm -rf "$publishDir"
fi

# Create publish directory
mkdir -p "$publishDir"

# Build for each platform
for runtime in "${runtimes[@]}"; do
    echo "Publishing for $runtime..."

    outputPath="$publishDir/$runtime"

    # Restore for the specific runtime first
    dotnet restore "$projectFile" -r "$runtime" /p:IsPublishing=true > /dev/null

    # Then publish
    if dotnet publish "$projectFile" \
        -c "$configuration" \
        -r "$runtime" \
        -o "$outputPath" \
        --no-restore \
        --self-contained true \
        /p:PublishSingleFile=true \
        /p:PublishTrimmed=false \
        /p:IsPublishing=true; then
        echo "✓ Successfully published for $runtime"
    else
        echo "✗ Failed to publish for $runtime"
    fi

    echo ""
done

echo "========================================"
echo "Build Complete!"
echo "Output directory: $publishDir"
echo "========================================"

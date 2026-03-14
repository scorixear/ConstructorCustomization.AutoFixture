param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot
try {
    Write-Host "Packing ConstructorCustomization.AutoFixture to ./artifacts..."
    dotnet pack ConstructorCustomization.AutoFixture/ConstructorCustomization.AutoFixture.csproj --configuration $Configuration --output ./artifacts
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }

    Write-Host "Restoring examples solution..."
    dotnet restore Examples/Examples.slnx --configfile Examples/nuget.config
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet restore failed with exit code $LASTEXITCODE"
    }

    Write-Host "Building examples solution..."
    dotnet build Examples/Examples.slnx --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }

    Write-Host "Completed: local package rebuilt and all examples compiled successfully."
}
finally {
    Pop-Location
}

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

    Write-Host "Restoring all projects..."
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet restore failed with exit code $LASTEXITCODE"
    }

    $exampleProjects = @(
        "Examples/Example.Net10/Example.Net10.csproj",
        "Examples/Example.Net8/Example.Net8.csproj",
        "Examples/Example.NetStandard21/Example.NetStandard21.csproj"
    )

    foreach ($project in $exampleProjects) {
        Write-Host "Building $project..."
        dotnet build $project --configuration $Configuration --no-restore
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet build failed for $project with exit code $LASTEXITCODE"
        }
    }

    Write-Host "Completed: local package rebuilt and all examples compiled successfully."
}
finally {
    Pop-Location
}

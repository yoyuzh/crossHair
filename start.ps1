param(
    [switch]$SkipBuild,
    [switch]$NoRestore,
    [switch]$Help
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Help) {
    Write-Host "Usage: .\start.ps1 [-SkipBuild] [-NoRestore]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -SkipBuild   Run the app without building first."
    Write-Host "  -NoRestore   Skip dotnet restore before building."
    exit 0
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$solution = Join-Path $root "CrosshairOverlay.sln"
$project = Join-Path $root "CrosshairOverlay.App\CrosshairOverlay.App.csproj"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet was not found. Install the .NET SDK first."
}

Push-Location $root
try {
    if (-not $NoRestore -and -not $SkipBuild) {
        dotnet restore $solution
    }

    if (-not $SkipBuild) {
        dotnet build $solution
    }

    dotnet run --project $project --no-restore
}
finally {
    Pop-Location
}

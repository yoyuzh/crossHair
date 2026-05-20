param(
    [ValidateSet("win-x64", "win-arm64")]
    [string]$Runtime = "win-x64",
    [string]$Configuration = "Release",
    [switch]$FrameworkDependent,
    [switch]$Help
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Help) {
    Write-Host "Usage: .\publish.ps1 [-Runtime win-x64|win-arm64] [-Configuration Release|Debug] [-FrameworkDependent]"
    Write-Host ""
    Write-Host "Default output:"
    Write-Host "  artifacts\publish\<runtime>\CrosshairOverlay.App.exe"
    exit 0
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root "CrosshairOverlay.App\CrosshairOverlay.App.csproj"
$output = Join-Path $root "artifacts\publish\$Runtime"
$selfContained = -not $FrameworkDependent

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet was not found. Install the .NET SDK first."
}

Push-Location $root
try {
    dotnet publish $project `
        -c $Configuration `
        -r $Runtime `
        --self-contained:$selfContained `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true `
        -o $output

    $exe = Join-Path $output "CrosshairOverlay.App.exe"
    if (-not (Test-Path $exe)) {
        Write-Error "Publish completed but exe was not found: $exe"
    }

    Write-Host "Published: $exe"
}
finally {
    Pop-Location
}

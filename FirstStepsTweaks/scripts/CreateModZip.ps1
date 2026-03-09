param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectDir,

    [Parameter(Mandatory = $true)]
    [string]$TargetPath,

    [Parameter(Mandatory = $true)]
    [string]$ModName,

    [Parameter(Mandatory = $true)]
    [string]$ModsFolder
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $TargetPath)) {
    throw "TargetPath not found: $TargetPath"
}

$targetDir = Split-Path -Path $TargetPath -Parent
if (-not (Test-Path -LiteralPath $targetDir)) {
    throw "Build output directory not found: $targetDir"
}

$stagingRoot = Join-Path -Path ([System.IO.Path]::GetTempPath()) -ChildPath ("{0}-staging" -f $ModName)
if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $stagingRoot | Out-Null

# Copy output files except external API references that should not be bundled.
Get-ChildItem -LiteralPath $targetDir -File | ForEach-Object {
    if ($_.Name -ieq 'VintagestoryAPI.dll') {
        return
    }

    Copy-Item -LiteralPath $_.FullName -Destination (Join-Path -Path $stagingRoot -ChildPath $_.Name) -Force
}

$assetsPath = Join-Path -Path $targetDir -ChildPath 'assets'
if (Test-Path -LiteralPath $assetsPath) {
    Copy-Item -LiteralPath $assetsPath -Destination (Join-Path -Path $stagingRoot -ChildPath 'assets') -Recurse -Force
}

if (-not (Test-Path -LiteralPath (Join-Path -Path $stagingRoot -ChildPath 'modinfo.json'))) {
    $fallbackModInfo = Join-Path -Path $ProjectDir -ChildPath 'modinfo.json'
    if (Test-Path -LiteralPath $fallbackModInfo) {
        Copy-Item -LiteralPath $fallbackModInfo -Destination (Join-Path -Path $stagingRoot -ChildPath 'modinfo.json') -Force
    }
}

if (-not (Test-Path -LiteralPath $ModsFolder)) {
    New-Item -ItemType Directory -Path $ModsFolder -Force | Out-Null
}

$zipPath = Join-Path -Path $ModsFolder -ChildPath ("{0}.zip" -f $ModName)
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path (Join-Path -Path $stagingRoot -ChildPath '*') -DestinationPath $zipPath -CompressionLevel Optimal -Force

Write-Host "Created mod zip: $zipPath"

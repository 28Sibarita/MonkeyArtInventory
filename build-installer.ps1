# Script para crear el instalador de MonkeyArtInventory usando Inno Setup
# Requiere tener Inno Setup instalado

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  Crear Instalador - MonkeyArtInventory     " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# Verificar que Inno Setup está instalado
$innoSetupPaths = @(
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    "C:\Program Files\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
)

$isccPath = $null
foreach ($path in $innoSetupPaths) {
    if (Test-Path $path) {
        $isccPath = $path
        break
    }
}

if (-not $isccPath) {
    Write-Host "ERROR: Inno Setup no está instalado" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor, descarga e instala Inno Setup desde:" -ForegroundColor Yellow
    Write-Host "https://jrsoftware.org/isdl.php" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Después de instalarlo, ejecuta este script nuevamente." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "ALTERNATIVA: Usa la versión portable" -ForegroundColor Green
    Write-Host "El ejecutable portable ya está disponible en: publish-windows-v2.0\" -ForegroundColor White
    exit 1
}

Write-Host "✓ Inno Setup encontrado en: $isccPath" -ForegroundColor Green
Write-Host ""

# Verificar que existe la publicación
$publishDir = "publish-windows-v2.0"
if (-not (Test-Path $publishDir)) {
    Write-Host "ERROR: No existe el directorio de publicación: $publishDir" -ForegroundColor Red
    Write-Host ""
    Write-Host "Primero ejecuta: .\publish-windows.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Publicación encontrada: $publishDir" -ForegroundColor Green
Write-Host ""

# Crear directorio de instaladores si no existe
$installerDir = "installers"
if (-not (Test-Path $installerDir)) {
    New-Item -ItemType Directory -Path $installerDir | Out-Null
}

# Compilar el instalador
$issFile = "installer\MonkeyArtInventory.iss"
if (-not (Test-Path $issFile)) {
    Write-Host "ERROR: No se encontró el script de Inno Setup: $issFile" -ForegroundColor Red
    exit 1
}

Write-Host ">> Compilando instalador..." -ForegroundColor Yellow
Write-Host "   Script: $issFile" -ForegroundColor Gray
Write-Host ""

& $isccPath $issFile

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: La compilación del instalador falló" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Green
Write-Host "  ✓ Instalador creado exitosamente          " -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""

# Buscar el instalador creado
$installerFile = Get-ChildItem -Path $installerDir -Filter "MonkeyArtInventory-Setup-*.exe" | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1

if ($installerFile) {
    $sizeMB = [Math]::Round($installerFile.Length / 1MB, 2)
    Write-Host "Instalador: $($installerFile.Name)" -ForegroundColor Cyan
    Write-Host "Tamaño: $sizeMB MB" -ForegroundColor Cyan
    Write-Host "Ubicación: $($installerFile.FullName)" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "El instalador está listo para distribuir!" -ForegroundColor Green
} else {
    Write-Host "ADVERTENCIA: No se encontró el instalador generado" -ForegroundColor Yellow
}

Write-Host ""

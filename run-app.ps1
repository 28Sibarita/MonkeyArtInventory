# Script para ejecutar MonkeyArtInventory
# Este script asegura que la aplicación se ejecute correctamente

Write-Host "=== MonkeyArtInventory - Script de Inicio ===" -ForegroundColor Cyan
Write-Host ""

# Detener procesos anteriores si existen
$existingProcesses = Get-Process | Where-Object {$_.ProcessName -like "*MonkeyArt*"}
if ($existingProcesses) {
    Write-Host "Deteniendo procesos anteriores..." -ForegroundColor Yellow
    $existingProcesses | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
}

# Navegar al directorio del proyecto
$projectPath = Join-Path $PSScriptRoot "src\MonkeyArtInventory.App\MonkeyArtInventory.App.csproj"

# Verificar que el proyecto existe
if (-not (Test-Path $projectPath)) {
    Write-Host "ERROR: No se encontró el proyecto en: $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host "Ejecutando MonkeyArtInventory..." -ForegroundColor Green
Write-Host "Proyecto: $projectPath" -ForegroundColor Gray
Write-Host ""

# Ejecutar la aplicación
dotnet run --project $projectPath

# Verificar si hubo errores
$logFile = Join-Path $env:TEMP "MonkeyArtInventory_app_start_error.log"
if (Test-Path $logFile) {
    Write-Host ""
    Write-Host "=== SE ENCONTRARON ERRORES ===" -ForegroundColor Red
    Get-Content $logFile
} else {
    Write-Host ""
    Write-Host "La aplicación se cerró correctamente." -ForegroundColor Green
}

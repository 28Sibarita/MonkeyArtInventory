# Script para publicar MonkeyArtInventory para Windows
# Genera un ejecutable único autocontenido

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  MonkeyArtInventory - Publicación Windows  " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"
$version = "2.0"
$publishDir = "publish-windows-v$version"
$projectPath = "src\MonkeyArtInventory.App\MonkeyArtInventory.App.csproj"

# Limpiar publicación anterior
if (Test-Path $publishDir) {
    Write-Host "Eliminando publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $publishDir
}

# Crear directorio de publicación
Write-Host "Creando directorio de publicación: $publishDir" -ForegroundColor Yellow
New-Item -ItemType Directory -Path $publishDir | Out-Null

# Publicar aplicación como ejecutable único
Write-Host ""
Write-Host ">> Publicando aplicación para Windows (x64)..." -ForegroundColor Green
Write-Host "   - Modo: Release" -ForegroundColor Gray
Write-Host "   - Runtime: win-x64" -ForegroundColor Gray
Write-Host "   - Autocontenido: Sí" -ForegroundColor Gray
Write-Host "   - Archivo único: Sí" -ForegroundColor Gray
Write-Host "   - ReadyToRun: Sí (optimización)" -ForegroundColor Gray
Write-Host ""

dotnet publish $projectPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishReadyToRun=true `
    -o $publishDir

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: La publicación falló" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✓ Publicación completada exitosamente" -ForegroundColor Green

# Verificar que el ejecutable se creó
$exePath = Join-Path $publishDir "MonkeyArtInventory.App.exe"
if (Test-Path $exePath) {
    $size = (Get-Item $exePath).Length / 1MB
    Write-Host ""
    Write-Host "Ejecutable creado: MonkeyArtInventory.App.exe" -ForegroundColor Cyan
    Write-Host "Tamaño: $([Math]::Round($size, 2)) MB" -ForegroundColor Cyan
    Write-Host "Ubicación: $publishDir" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "ADVERTENCIA: No se encontró el ejecutable" -ForegroundColor Yellow
}

# Copiar icono si existe
$iconSource = "publish\MonkeyArtInventory\app.ico"
if (Test-Path $iconSource) {
    Write-Host ""
    Write-Host "Copiando icono de aplicación..." -ForegroundColor Yellow
    Copy-Item $iconSource -Destination $publishDir -Force
}

# Crear README para la publicación
Write-Host "Creando archivo README..." -ForegroundColor Yellow

$readmeContent = @"
# MonkeyArtInventory - Versión $version para Windows

## Instalación

Esta es una versión portable que no requiere instalación.

### Opción 1: Ejecutar directamente (Portable)
1. Haz doble clic en **MonkeyArtInventory.App.exe**
2. La aplicación se ejecutará inmediatamente

### Opción 2: Crear instalador
1. Usa el instalador **MonkeyArtInventory-Setup-v$version.exe** (si está disponible)
2. Sigue el asistente de instalación
3. Se creará un acceso directo en el escritorio

## Características

- ✓ Ejecutable único autocontenido
- ✓ No requiere .NET instalado
- ✓ No requiere instalación (versión portable)
- ✓ Datos guardados en: %LocalAppData%\MonkeyArtInventory

## Requisitos del Sistema

- Windows 10 o superior (x64)
- 100 MB de espacio en disco
- No requiere permisos de administrador

## Ubicación de Datos

Los datos de la aplicación se guardan en:
``````
C:\Users\[TuUsuario]\AppData\Local\MonkeyArtInventory\
``````

Dentro de esta carpeta encontrarás:
- **Data\** - Base de datos SQLite
- **Reportes\** - Reportes generados
- **Logs\** - Archivos de log

## Solución de Problemas

### La aplicación no inicia
1. Asegúrate de tener Windows 10 o superior
2. Verifica que tienes permisos de ejecución
3. Revisa el log de errores en: %TEMP%\MonkeyArtInventory_app_start_error.log

### Cómo desinstalar (versión portable)
Simplemente elimina la carpeta donde está el ejecutable.
Los datos de la aplicación permanecerán en %LocalAppData%\MonkeyArtInventory\ 
por si deseas reinstalar más tarde.

## Soporte

- GitHub: https://github.com/28Sibarita/MonkeyArtInventory
- Issues: https://github.com/28Sibarita/MonkeyArtInventory/issues

---
Versión: $version
Fecha: $(Get-Date -Format "dd/MM/yyyy")
"@

$readmeContent | Out-File -FilePath "$publishDir\README.txt" -Encoding UTF8

Write-Host ""
Write-Host "=============================================" -ForegroundColor Green
Write-Host "  ✓ Publicación completada exitosamente     " -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Archivos generados en: $publishDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "Siguiente paso:" -ForegroundColor Yellow
Write-Host "  1. Prueba el ejecutable: .\$publishDir\MonkeyArtInventory.App.exe" -ForegroundColor White
Write-Host "  2. Para crear instalador, ejecuta: .\build-installer.ps1" -ForegroundColor White
Write-Host ""

# Script para publicar la version Mac/Linux de MonkeyArtInventory
# Ejecutar desde la raiz del proyecto

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "MonkeyArtInventory - Build Mac/Linux" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Crear directorio de publicacion
$publishDir = "publish-crossplatform"
if (Test-Path $publishDir) {
    Remove-Item -Recurse -Force $publishDir
}
New-Item -ItemType Directory -Path $publishDir | Out-Null

# Publicar para macOS (x64)
Write-Host "`n>> Compilando para macOS (x64)..." -ForegroundColor Yellow
dotnet publish src/MonkeyArtInventory.Mac/MonkeyArtInventory.Mac.csproj `
    -c Release `
    -r osx-x64 `
    --self-contained `
    -o "$publishDir/MonkeyArtInventory-macOS-x64"

# Publicar para macOS (ARM64 - Apple Silicon)
Write-Host "`n>> Compilando para macOS (ARM64 - Apple Silicon)..." -ForegroundColor Yellow
dotnet publish src/MonkeyArtInventory.Mac/MonkeyArtInventory.Mac.csproj `
    -c Release `
    -r osx-arm64 `
    --self-contained `
    -o "$publishDir/MonkeyArtInventory-macOS-arm64"

# Publicar para Linux (x64)
Write-Host "`n>> Compilando para Linux (x64)..." -ForegroundColor Yellow
dotnet publish src/MonkeyArtInventory.Mac/MonkeyArtInventory.Mac.csproj `
    -c Release `
    -r linux-x64 `
    --self-contained `
    -o "$publishDir/MonkeyArtInventory-Linux-x64"

# Crear scripts de ejecucion para cada plataforma
Write-Host "`n>> Creando scripts de ejecucion..." -ForegroundColor Yellow

# Script para macOS x64
@"
#!/bin/bash
cd "`$(dirname "`$0")"
./MonkeyArtInventory.Mac
"@ | Out-File -FilePath "$publishDir/MonkeyArtInventory-macOS-x64/run-app.sh" -Encoding utf8

# Script para macOS ARM64
@"
#!/bin/bash
cd "`$(dirname "`$0")"
./MonkeyArtInventory.Mac
"@ | Out-File -FilePath "$publishDir/MonkeyArtInventory-macOS-arm64/run-app.sh" -Encoding utf8

# Script para Linux
@"
#!/bin/bash
cd "`$(dirname "`$0")"
./MonkeyArtInventory.Mac
"@ | Out-File -FilePath "$publishDir/MonkeyArtInventory-Linux-x64/run-app.sh" -Encoding utf8

# Crear README con instrucciones
@"
# MonkeyArtInventory - Version Multiplataforma

## Instrucciones de instalacion

### macOS
1. Descarga la carpeta correspondiente a tu Mac:
   - **MonkeyArtInventory-macOS-x64**: Para Mac Intel
   - **MonkeyArtInventory-macOS-arm64**: Para Mac con Apple Silicon (M1, M2, etc.)

2. Abre Terminal y navega a la carpeta descargada

3. Da permisos de ejecucion al script:
   ``````
   chmod +x run-app.sh MonkeyArtInventory.Mac
   ``````

4. Ejecuta la aplicacion:
   ``````
   ./run-app.sh
   ``````

### Linux
1. Descarga la carpeta **MonkeyArtInventory-Linux-x64**

2. Abre Terminal y navega a la carpeta descargada

3. Da permisos de ejecucion:
   ``````
   chmod +x run-app.sh MonkeyArtInventory.Mac
   ``````

4. Ejecuta la aplicacion:
   ``````
   ./run-app.sh
   ``````

## Nota
La base de datos (SQLite) se crea automaticamente en el mismo directorio de la aplicacion.
"@ | Out-File -FilePath "$publishDir/README.md" -Encoding utf8

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host "Build completado!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Archivos en: $publishDir" -ForegroundColor Cyan
Write-Host "`n  - MonkeyArtInventory-macOS-x64   (Mac Intel)"
Write-Host "  - MonkeyArtInventory-macOS-arm64 (Mac M1/M2)"
Write-Host "  - MonkeyArtInventory-Linux-x64   (Linux)"

# Mostrar tamano de carpetas
Write-Host "`nTamanos:" -ForegroundColor Yellow
Get-ChildItem -Path $publishDir -Directory | ForEach-Object {
    $size = (Get-ChildItem -Path $_.FullName -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host ("  {0}: {1:N1} MB" -f $_.Name, $size)
}

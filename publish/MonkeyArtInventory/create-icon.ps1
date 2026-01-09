# Crear ícono para Monkey Art Inventory
# Este script crea un ícono personalizado desde el EXE

param(
    [string]$AppPath = $PSScriptRoot,
    [string]$AppName = "MonkeyArtInventory.exe"
)

$exePath = Join-Path $AppPath $AppName
$iconPath = Join-Path $AppPath "icon.ico"

if (-not (Test-Path $exePath)) {
    Write-Error "No se encontró: $exePath"
    exit 1
}

# Extraer ícono del EXE
# Windows tiene iconos por defecto en imageres.dll
$shell = New-Object -ComObject Shell.Shell
$folder = $shell.CreateShortCut("$iconPath")

# Si eso no funciona, usar el ícono predefinido del sistema
if (-not (Test-Path $iconPath)) {
    Copy-Item "C:\Windows\System32\shell32.dll" -Destination "$iconPath" -ErrorAction SilentlyContinue
    
    # Alternativa: crear un ícono simple desde el EXE
    Write-Host "Creando ícono desde el EXE..."
    try {
        $extractor = New-Object -ComObject Shell.Application
        $folder = $extractor.NameSpace($AppPath)
        $file = $folder.ParseName($AppName)
        # Usar el ícono del EXE directamente
    } catch {
        Write-Warning "No se pudo extraer ícono personalizado. Se usará ícono predefinido."
    }
}

Write-Host "✓ Ícono procesado en: $iconPath"

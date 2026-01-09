@echo off
REM Script para instalar Monkey Art Inventory en el menú de inicio

setlocal enabledelayedexpansion

REM Obtener ruta actual
set SCRIPT_DIR=%~dp0
set APP_EXE=%SCRIPT_DIR%MonkeyArtInventory.exe

REM Verificar que el EXE existe
if not exist "%APP_EXE%" (
    echo ERROR: No se encontró %APP_EXE%
    pause
    exit /b 1
)

REM Crear carpeta de StartMenu si no existe
set START_MENU=%APPDATA%\Microsoft\Windows\Start Menu\Programs
if not exist "%START_MENU%" mkdir "%START_MENU%"

REM Crear el atajo usando PowerShell (más confiable)
powershell -Command ^
    "$WshShell = New-Object -ComObject WScript.Shell; " ^
    "$shortcut = $WshShell.CreateShortCut('%START_MENU%\Monkey Art Inventory.lnk'); " ^
    "$shortcut.TargetPath = '%APP_EXE%'; " ^
    "$shortcut.WorkingDirectory = '%SCRIPT_DIR%'; " ^
    "$shortcut.Description = 'Monkey Art Inventory - Gestión de inventario de arte'; " ^
    "$shortcut.WindowStyle = 1; " ^
    "$shortcut.Save()"

if errorlevel 1 (
    echo ERROR: No se pudo crear el atajo
    pause
    exit /b 1
)

echo ✓ ¡Atajo creado exitosamente en el menú de inicio!
echo   Puedes encontrarlo en: Menú de inicio (búsqueda: "Monkey Art")
pause

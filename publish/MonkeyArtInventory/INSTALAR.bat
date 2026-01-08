@echo off
title Instalador - Monkey Art Inventory
color 0A

echo.
echo  ========================================
echo   INSTALADOR - Monkey Art Inventory
echo  ========================================
echo.

REM Verificar que el EXE existe
set SCRIPT_DIR=%~dp0
set APP_EXE=%SCRIPT_DIR%MonkeyArtInventory.exe

if not exist "%APP_EXE%" (
    color 0C
    echo ERROR: No se encontro MonkeyArtInventory.exe
    echo Asegurate de ejecutar este instalador desde la carpeta extraida.
    pause
    exit /b 1
)

echo Instalando Monkey Art Inventory...
echo.

REM Crear carpeta de instalacion en AppData (no requiere admin)
set INSTALL_DIR=%LOCALAPPDATA%\MonkeyArtInventory\App
echo [1/4] Creando carpeta de instalacion...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

REM Copiar todos los archivos
echo [2/4] Copiando archivos (esto puede tardar un momento)...
xcopy /E /Y /Q "%SCRIPT_DIR%*" "%INSTALL_DIR%\" >nul 2>&1
if errorlevel 1 (
    color 0C
    echo ERROR: No se pudieron copiar los archivos.
    pause
    exit /b 1
)

REM Crear acceso directo en Escritorio
echo [3/4] Creando acceso directo en Escritorio...
set DESKTOP=%USERPROFILE%\Desktop
powershell -Command ^
    "$WshShell = New-Object -ComObject WScript.Shell; " ^
    "$shortcut = $WshShell.CreateShortCut('%DESKTOP%\Monkey Art Inventory.lnk'); " ^
    "$shortcut.TargetPath = '%INSTALL_DIR%\MonkeyArtInventory.exe'; " ^
    "$shortcut.WorkingDirectory = '%INSTALL_DIR%'; " ^
    "$shortcut.Description = 'Monkey Art Inventory - Gestion de inventario'; " ^
    "$shortcut.IconLocation = '%INSTALL_DIR%\app.ico,0'; " ^
    "$shortcut.Save()"

REM Crear acceso directo en Menu de Inicio
echo [4/4] Creando acceso directo en Menu de Inicio...
set START_MENU=%APPDATA%\Microsoft\Windows\Start Menu\Programs
powershell -Command ^
    "$WshShell = New-Object -ComObject WScript.Shell; " ^
    "$shortcut = $WshShell.CreateShortCut('%START_MENU%\Monkey Art Inventory.lnk'); " ^
    "$shortcut.TargetPath = '%INSTALL_DIR%\MonkeyArtInventory.exe'; " ^
    "$shortcut.WorkingDirectory = '%INSTALL_DIR%'; " ^
    "$shortcut.Description = 'Monkey Art Inventory - Gestion de inventario'; " ^
    "$shortcut.IconLocation = '%INSTALL_DIR%\app.ico,0'; " ^
    "$shortcut.Save()"

echo.
echo  ========================================
color 0A
echo   INSTALACION COMPLETADA!
echo  ========================================
echo.
echo   - Icono creado en el Escritorio
echo   - Icono creado en el Menu de Inicio
echo   - Programa instalado en:
echo     %INSTALL_DIR%
echo.
echo   Puedes eliminar esta carpeta de descarga.
echo.
echo  ========================================
echo.

set /p LAUNCH="Deseas abrir la aplicacion ahora? (S/N): "
if /i "%LAUNCH%"=="S" (
    start "" "%INSTALL_DIR%\MonkeyArtInventory.exe"
)

echo.
echo Presiona cualquier tecla para cerrar...
pause >nul

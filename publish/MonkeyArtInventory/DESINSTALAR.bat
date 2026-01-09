@echo off
title Desinstalador - Monkey Art Inventory
color 0E

echo.
echo  ========================================
echo   DESINSTALADOR - Monkey Art Inventory
echo  ========================================
echo.

set INSTALL_DIR=%LOCALAPPDATA%\MonkeyArtInventory\App
set DESKTOP=%USERPROFILE%\Desktop
set START_MENU=%APPDATA%\Microsoft\Windows\Start Menu\Programs

echo Desinstalando Monkey Art Inventory...
echo.

REM Eliminar acceso directo del Escritorio
echo [1/3] Eliminando acceso directo del Escritorio...
if exist "%DESKTOP%\Monkey Art Inventory.lnk" del "%DESKTOP%\Monkey Art Inventory.lnk"

REM Eliminar acceso directo del Menu de Inicio
echo [2/3] Eliminando acceso directo del Menu de Inicio...
if exist "%START_MENU%\Monkey Art Inventory.lnk" del "%START_MENU%\Monkey Art Inventory.lnk"

REM Eliminar carpeta de instalacion
echo [3/3] Eliminando archivos del programa...
if exist "%INSTALL_DIR%" rmdir /S /Q "%INSTALL_DIR%"

echo.
echo  ========================================
color 0A
echo   DESINSTALACION COMPLETADA!
echo  ========================================
echo.
echo   Nota: Los datos de usuario se conservan en:
echo   %LOCALAPPDATA%\MonkeyArtInventory\Data
echo.
echo   Si deseas eliminarlos tambien, borra esa carpeta manualmente.
echo.

pause

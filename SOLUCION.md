# Solución al Problema de Ejecución - MonkeyArtInventory

## Problema Encontrado

La aplicación no se ejecutaba debido a un error en la inicialización de la base de datos. El error específico era:

```
SQLite Error 1: 'no such table: Products'
```

## Causa del Problema

El código en `DatabaseInitializer.cs` tenía una lógica incorrecta:
1. Intentaba aplicar migraciones
2. Si fallaba, ejecutaba `EnsureCreatedAsync()` dos veces
3. Entity Framework no puede usar `EnsureCreatedAsync()` cuando ya existen migraciones definidas
4. Esto causaba que la base de datos se creara sin tablas

## Solución Implementada

### 1. Se corrigió el archivo `DatabaseInitializer.cs`

**Antes:**
```csharp
try
{
    await dbContext.Database.MigrateAsync();
}
catch
{
    await dbContext.Database.EnsureCreatedAsync();
}
await dbContext.Database.EnsureCreatedAsync(); // ← Llamada duplicada y problemática
```

**Después:**
```csharp
bool migrationApplied = false;
try
{
    await dbContext.Database.MigrateAsync();
    migrationApplied = true;
}
catch
{
    // If migrations fail, do nothing here - we'll try EnsureCreated below
}

if (!migrationApplied)
{
    await dbContext.Database.EnsureCreatedAsync();
}
```

### 2. Se aplicaron las migraciones manualmente

Se utilizó la herramienta `ApplyMigrations` para crear correctamente la base de datos:

```powershell
dotnet run --project "tools\ApplyMigrations\ApplyMigrations.csproj"
```

## Cómo Ejecutar la Aplicación

### Opción 1: Usar el script de inicio (Recomendado)

```powershell
.\run-app.ps1
```

### Opción 2: Ejecutar manualmente

```powershell
dotnet run --project "src\MonkeyArtInventory.App\MonkeyArtInventory.App.csproj"
```

### Opción 3: Desde el directorio del proyecto

```powershell
cd src\MonkeyArtInventory.App
dotnet run
```

## Ubicación de la Base de Datos

La base de datos SQLite se encuentra en:
```
%LocalAppData%\MonkeyArtInventory\Data\inventory.db
```

Ruta completa típica:
```
C:\Users\[TuUsuario]\AppData\Local\MonkeyArtInventory\Data\inventory.db
```

## Logs de Errores

Si la aplicación falla al iniciar, revisa el archivo de log:
```
%TEMP%\MonkeyArtInventory_app_start_error.log
```

## Solución de Problemas

### Si la aplicación no inicia:

1. **Verificar que no hay procesos anteriores:**
   ```powershell
   Get-Process | Where-Object {$_.ProcessName -like "*MonkeyArt*"} | Stop-Process -Force
   ```

2. **Eliminar la base de datos y recrearla:**
   ```powershell
   Remove-Item "$env:LOCALAPPDATA\MonkeyArtInventory\Data\inventory.db" -Force
   dotnet run --project "tools\ApplyMigrations\ApplyMigrations.csproj"
   ```

3. **Verificar el log de errores:**
   ```powershell
   Get-Content "$env:TEMP\MonkeyArtInventory_app_start_error.log"
   ```

## Cambios Realizados

### Archivos Modificados:
- `src\MonkeyArtInventory.Data\DatabaseInitializer.cs` - Corregida la lógica de inicialización de base de datos

### Archivos Creados:
- `run-app.ps1` - Script para facilitar la ejecución de la aplicación
- `SOLUCION.md` - Este archivo con la documentación de la solución

## Compilación

Para compilar la solución completa:

```powershell
dotnet build
```

Para compilar solo la aplicación principal:

```powershell
dotnet build src\MonkeyArtInventory.App\MonkeyArtInventory.App.csproj
```

## Estado Actual

✓ La aplicación ahora se ejecuta correctamente
✓ La base de datos se crea automáticamente en el primer inicio
✓ Las migraciones se aplican correctamente
✓ Los datos de ejemplo se insertan automáticamente

---

## 🚀 PUBLICACIÓN v2.0

### ✅ Compilación Exitosa

El ejecutable portable ha sido generado correctamente:

- **Archivo:** `publish-windows-v2.0\MonkeyArtInventory.App.exe`
- **Tamaño:** 93.25 MB
- **Tipo:** Ejecutable único autocontenido
- **No requiere:** .NET instalado ni instalación previa

### 📤 Para Subir a GitHub

Ver instrucciones completas en: [INSTRUCCIONES-GITHUB.md](./INSTRUCCIONES-GITHUB.md)

**Pasos rápidos:**
1. Ve a: https://github.com/28Sibarita/MonkeyArtInventory/releases
2. Click en "Draft a new release"
3. Tag: `v2.0`
4. Sube el archivo: `publish-windows-v2.0\MonkeyArtInventory.App.exe`
5. Publica la release

### 📚 Documentación Adicional

- [GUIA-PUBLICACION.md](./GUIA-PUBLICACION.md) - Guía completa de publicación
- [INSTRUCCIONES-GITHUB.md](./INSTRUCCIONES-GITHUB.md) - Pasos detallados para GitHub

---

**Fecha de solución:** 9 de enero de 2026
**Versión de .NET:** 8.0
**Framework de UI:** WPF (Windows Presentation Foundation)
**Versión publicada:** v2.0

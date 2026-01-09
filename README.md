# 🎨 MonkeyArtInventory

Sistema de inventario offline para MonkeyArt. Disponible para **Windows**, **macOS** y **Android**.

---

## 📥 Descargas v2.0 (Última versión) ⭐

### 🪟 Windows - Versión Portable (Recomendada)

**[⬇️ Descargar MonkeyArtInventory v2.0 (93 MB)](https://github.com/28Sibarita/MonkeyArtInventory/releases/download/v2.0/MonkeyArtInventory.App.exe)**

- ✅ **No requiere instalación** - Ejecuta directamente
- ✅ **No requiere .NET** - Todo incluido en un solo archivo
- ✅ **Corrección crítica** - Bug de base de datos resuelto
- 💻 Requisitos: Windows 10+ (64-bit)

### Otras Plataformas (v1.1)

| Plataforma | Archivo | Tamaño | Requisitos |
|------------|---------|--------|------------|
| **🍎 macOS** | [MonkeyArtInventory-Mac-v1.1.zip](https://github.com/28Sibarita/MonkeyArtInventory/releases/download/v1.1/MonkeyArtInventory-Mac-v1.1.zip) | 98 MB | Ninguno (self-contained) |
| **🤖 Android** | [MonkeyArtInventory-Android-v1.1.zip](https://github.com/28Sibarita/MonkeyArtInventory/releases/download/v1.1/MonkeyArtInventory-Android-v1.1.zip) | 125 MB | Android 5.0+ |

---

## 🎉 Novedades en v2.0

- 🐛 **Corrección crítica**: Error "no such table: Products" resuelto
- ✨ **Ejecutable portable**: Un solo archivo, sin instalación
- 🚀 **Auto-contenido**: No requiere .NET instalado
- 📖 **Documentación mejorada**: Guías completas incluidas

## ✨ Características

- 📦 **Gestión de inventario** - Productos con código de barras, stock mínimo, ubicación
- 👥 **Clientes/Destinos** - Registra a dónde van los productos
- 📥📤 **Movimientos** - Entradas y salidas con destino/cliente
- 📊 **Dashboard** - Resumen visual del inventario
- 📈 **Reportes** - Exportación a Excel y PDF
- 💾 **Base de datos local** - SQLite, funciona sin internet

---

## 🚀 Instalación

### Windows
1. Descomprime el ZIP
2. Ejecuta `MonkeyArtInventory.App.exe`
3. Si no tienes .NET 8, descárgalo de [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)

### macOS
1. Descomprime el ZIP (contiene versiones para Intel y Apple Silicon)
2. Ejecuta el archivo `MonkeyArtInventory.Mac`
3. Si macOS lo bloquea: Preferencias del Sistema → Seguridad → "Abrir de todos modos"

### Android
1. Descomprime el ZIP
2. Copia el APK al dispositivo
3. Habilita "Instalar de fuentes desconocidas"
4. Instala el APK

---

## 🛠️ Desarrollo

### Requirements
- .NET 8 SDK

## Run in development
1. Open `MonkeyArtInventory.sln`
2. Set startup project to `MonkeyArtInventory.App`
3. Run (F5)

## Build self-contained (example)
From the solution root:

```powershell
cd src\MonkeyArtInventory.App
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Output will be under `bin\Release\net8.0-windows\win-x64\publish`.

## CLI weekly report
The app executable supports:

- `--weekly-report`
- `--export-weekly-report`
- `--report-output "C:\\path\\to\\folder"`

Example:

```powershell
MonkeyArtInventory.App.exe --weekly-report --report-output "C:\\Reports"
```

## Scheduled task
Use `create_task.ps1` from the solution root. Example:

```powershell
.\create_task.ps1 -ExePath "C:\\Path\\To\\MonkeyArtInventory.App.exe"
```

To run whether the user is logged on or not:

```powershell
.\create_task.ps1 -ExePath "C:\\Path\\To\\MonkeyArtInventory.App.exe" -RunWhenLoggedOff
```

Logs are written to:
`%LOCALAPPDATA%\MonkeyArtInventory\Logs\weekly_report.log`

## Data locations
Default paths:
- Database: `%LOCALAPPDATA%\MonkeyArtInventory\Data\inventory.db`
- Reports: `%LOCALAPPDATA%\MonkeyArtInventory\Reportes`
- Logs: `%LOCALAPPDATA%\MonkeyArtInventory\Logs`
- Backups: `%LOCALAPPDATA%\MonkeyArtInventory\Backups`

## Settings
Settings are stored at:
`%LOCALAPPDATA%\MonkeyArtInventory\settings.json`

You can change:
- Data and report paths
- Enable or disable prices
- Backup retention count

## Notes
- QuestPDF is used for PDF generation. Review its license terms for production use.

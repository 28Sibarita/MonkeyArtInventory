# Monkey Art Inventory - Guía de Instalación

## Requisitos
- Windows 7 o superior (x64)
- No requiere instalación de .NET o dependencias adicionales

## Instalación

1. **Descargar**: Descarga y extrae `MonkeyArtInventory.zip`

2. **Opción A - Ejecución directa** (rápido):
   - Ve a la carpeta extraída
   - Abre `MonkeyArtInventory.exe` (doble click)

3. **Opción B - Instalar en Menú de Inicio** (recomendado):
   - Ve a la carpeta extraída
   - Click derecho en `install-shortcut.bat`
   - Selecciona "Ejecutar como administrador"
   - Se creará un acceso directo en tu Menú de Inicio
   - Luego busca "Monkey Art" en el Menú de Inicio o usa la tecla Windows

¡Listo! La aplicación se abrirá

## Primera ejecución

- La app crea automáticamente:
  - Base de datos en: `%LocalAppData%\MonkeyArtInventory\Data\inventory.db`
  - Configuración en: `%LocalAppData%\MonkeyArtInventory\settings.json`
  - Reportes en: `%LocalAppData%\MonkeyArtInventory\Reportes\`

## Características

- ✅ Gestión de inventario de arte
- ✅ Registro de movimientos (entrada/salida)
- ✅ Seguimiento de clientes y consignaciones
- ✅ Reportes semanales en PDF y Excel
- ✅ Gráficos de stock y ventas
- ✅ Backup automático

## Uso desde línea de comandos

```powershell
# Generar informe semanal
MonkeyArtInventory.exe --weekly-report --report-output "C:\Mis Reportes"
```

## Problemas comunes

**Mensaje: "API not available"**
- Algunos gráficos requieren Windows 10+. Actualiza tu Windows.

**La app se cierra sin abrir**
- Revisa `%TEMP%\MonkeyArtInventory_app_start_error.log`
- Asegúrate de tener permisos de escritura en `%LocalAppData%`

## Soporte

Para reportar problemas o solicitar funciones:
https://github.com/28Sibarita/MonkeyArtInventory

---

**Versión**: 1.0  
**Última actualización**: Enero 2026

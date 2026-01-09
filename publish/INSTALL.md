# Monkey Art Inventory - Guía de Instalación

## Requisitos
- Windows 7 o superior (x64)
- No requiere instalación de .NET o dependencias adicionales

## Instalación

### Instalación rápida (recomendado)

1. **Descargar**: Descarga y extrae `MonkeyArtInventory.zip`
2. **Instalar**: 
   - Abre la carpeta extraída
   - **Doble click en `INSTALAR.bat`**
   - Espera a que termine (copia archivos, crea íconos)
3. **¡Listo!** 
   - Aparecerá un ícono en tu **Escritorio**
   - También en el **Menú de Inicio** (busca "Monkey Art")

### Ejecución sin instalar

Si solo quieres probar sin instalar:
- Abre `MonkeyArtInventory.exe` directamente (doble click)

### Desinstalación

- Ejecuta `DESINSTALAR.bat` desde la carpeta original
- O elimina manualmente el ícono del escritorio y la carpeta `%LOCALAPPDATA%\MonkeyArtInventory`

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

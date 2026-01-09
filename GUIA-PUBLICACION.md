# Guía Completa de Publicación - MonkeyArtInventory v2.0

Esta guía te ayudará a publicar MonkeyArtInventory en GitHub con archivos descargables e instalables.

## Pasos Completos

### 1. Publicar la Aplicación para Windows

Ejecuta el script de publicación que creará un ejecutable único autocontenido:

```powershell
.\publish-windows.ps1
```

Este script:
- Compila la aplicación en modo Release
- Crea un ejecutable único (MonkeyArtInventory.App.exe) de ~60-100 MB
- No requiere .NET instalado para ejecutarse
- Guarda todo en la carpeta `publish-windows-v2.0\`

**Resultado:** Un archivo `.exe` portable que se puede ejecutar sin instalación.

---

### 2. Crear el Instalador (Opcional pero Recomendado)

#### 2.1 Instalar Inno Setup

Si no tienes Inno Setup instalado:

1. Descarga desde: https://jrsoftware.org/isdl.php
2. Instala la versión más reciente (6.x)
3. Usa la configuración predeterminada

#### 2.2 Compilar el Instalador

```powershell
.\build-installer.ps1
```

Este script:
- Verifica que Inno Setup esté instalado
- Compila el script `installer\MonkeyArtInventory.iss`
- Genera un instalador en la carpeta `installers\`
- El instalador se llama: `MonkeyArtInventory-Setup-v2.0.exe`

**Resultado:** Un instalador profesional de ~40-60 MB que:
- Instala la aplicación en `%LocalAppData%\MonkeyArtInventory\App`
- Crea acceso directo en el escritorio
- Crea entrada en el menú inicio
- Permite desinstalar desde "Programas y características"

---

### 3. Subir a GitHub

#### Opción A: Con Git Instalado (Recomendado)

```powershell
.\setup-github.ps1
```

Este script:
1. Configura Git si es necesario
2. Crea el archivo `.gitignore`
3. Hace commit de los cambios
4. Crea un tag `v2.0`
5. Sube todo a GitHub

Luego:
1. Ve a: https://github.com/28Sibarita/MonkeyArtInventory/releases
2. Click en "Draft a new release"
3. Selecciona el tag `v2.0`
4. Sube como assets:
   - `publish-windows-v2.0\MonkeyArtInventory.App.exe` (ejecutable portable)
   - `installers\MonkeyArtInventory-Setup-v2.0.exe` (instalador)

#### Opción B: Sin Git (Manual)

1. **Instalar Git:**
   - Descarga desde: https://git-scm.com/download/win
   - O usa GitHub Desktop: https://desktop.github.com/

2. **Subir manualmente vía Web:**
   - Ve a https://github.com/28Sibarita/MonkeyArtInventory
   - Click en "Add file" > "Upload files"
   - Arrastra los archivos modificados
   - Haz commit

3. **Crear Release:**
   - Ve a la pestaña "Releases"
   - Click "Draft a new release"
   - Tag: `v2.0`
   - Title: `MonkeyArtInventory v2.0`
   - Descripción (ejemplo):

```markdown
# MonkeyArtInventory v2.0

## 🎉 Novedades

- ✓ Corrección crítica en la inicialización de base de datos
- ✓ Mejora en el sistema de migraciones
- ✓ Scripts de publicación optimizados
- ✓ Instalador profesional incluido
- ✓ Versión portable disponible

## 📦 Descargas

### Para Usuarios Finales:

**Opción 1: Instalador (Recomendado)**
- Descarga: `MonkeyArtInventory-Setup-v2.0.exe`
- Tamaño: ~40-60 MB
- Instalación automática con accesos directos

**Opción 2: Versión Portable**
- Descarga: `MonkeyArtInventory.App.exe`
- Tamaño: ~60-100 MB
- No requiere instalación, ejecuta directamente

### Requisitos del Sistema:
- Windows 10 o superior (64-bit)
- 100 MB de espacio libre
- No requiere .NET instalado

## 🐛 Correcciones

- Corregido error "no such table: Products" al iniciar
- Mejorada la lógica de inicialización de base de datos
- Aplicación de migraciones más robusta

## 📖 Documentación

Ver [SOLUCION.md](./SOLUCION.md) para detalles técnicos de las correcciones.
```

4. **Subir archivos:**
   - Arrastra o selecciona:
     - `MonkeyArtInventory.App.exe` (portable)
     - `MonkeyArtInventory-Setup-v2.0.exe` (instalador)
   
5. Click "Publish release"

---

## Estructura Final en GitHub

```
MonkeyArtInventory/
├── Releases (pestaña)
│   └── v2.0
│       ├── MonkeyArtInventory.App.exe (portable)
│       └── MonkeyArtInventory-Setup-v2.0.exe (instalador)
│
├── src/
│   ├── MonkeyArtInventory.App/
│   ├── MonkeyArtInventory.Core/
│   └── MonkeyArtInventory.Data/
│
├── installer/
│   └── MonkeyArtInventory.iss
│
├── publish-windows.ps1
├── build-installer.ps1
├── setup-github.ps1
├── run-app.ps1
├── README.md
└── SOLUCION.md
```

---

## Verificación Final

Después de publicar, verifica:

1. ✓ Los archivos están disponibles en la release
2. ✓ Los enlaces de descarga funcionan
3. ✓ El ejecutable portable funciona en una PC limpia
4. ✓ El instalador instala correctamente
5. ✓ La aplicación inicia sin errores

---

## Comandos Rápidos de Referencia

```powershell
# Publicar para Windows
.\publish-windows.ps1

# Crear instalador
.\build-installer.ps1

# Configurar y subir a GitHub
.\setup-github.ps1

# Ejecutar aplicación localmente
.\run-app.ps1

# Ver errores si falla
Get-Content "$env:TEMP\MonkeyArtInventory_app_start_error.log"
```

---

## Solución de Problemas Comunes

### Error: "Inno Setup no encontrado"
- Instala desde: https://jrsoftware.org/isdl.php
- O salta el paso del instalador y usa solo la versión portable

### Error: "Git no reconocido"
- Instala Git desde: https://git-scm.com/download/win
- O usa la opción manual para subir a GitHub

### Error al compilar
- Verifica que tenés .NET 8.0 SDK instalado
- Ejecuta: `dotnet --version`
- Si no está instalado: https://dotnet.microsoft.com/download

### Archivos muy grandes
- Es normal, el ejecutable autocontenido incluye el runtime de .NET
- El instalador comprime mejor los archivos

---

## Próximos Pasos

Después de publicar la v2.0:

1. Compartir el enlace de release con usuarios
2. Actualizar el README.md principal con el enlace de descarga
3. Considerar automatización con GitHub Actions (futuro)
4. Recopilar feedback de usuarios

---

**Fecha:** 9 de enero de 2026  
**Versión:** 2.0  
**Autor:** MonkeyArt Team

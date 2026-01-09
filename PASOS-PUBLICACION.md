# 🚀 INSTRUCCIONES SIMPLES - SUBIR A GITHUB

## ⚡ Pasos Rápidos (5 minutos)

### 📋 ARCHIVOS LISTOS PARA SUBIR:

1. **Ejecutable Principal:**
   - `publish-windows-v2.0\MonkeyArtInventory.App.exe` (93 MB)
   - O usa: `releases\MonkeyArtInventory-v2.0-Windows-Portable.zip` (87 MB) ⭐

2. **Código Fuente Modificado:**
   - `src\MonkeyArtInventory.Data\DatabaseInitializer.cs`
   - `README.md`
   - `SOLUCION.md`
   - `GUIA-PUBLICACION.md`
   - `publish-windows.ps1`

---

## 🎯 PASOS PARA PUBLICAR

### PASO 1: Actualizar el Código en GitHub

#### Opción A: Subir archivo por archivo (Más fácil)

1. **Ve a tu repositorio:**
   ```
   https://github.com/28Sibarita/MonkeyArtInventory
   ```

2. **Actualizar DatabaseInitializer.cs:**
   - Navega a: `src` → `MonkeyArtInventory.Data` → `DatabaseInitializer.cs`
   - Click en el ícono del **lápiz** (Edit)
   - Borra todo el contenido
   - Copia y pega el contenido de tu archivo local:
     `src\MonkeyArtInventory.Data\DatabaseInitializer.cs`
   - Mensaje de commit: `Fix: Corrección crítica en DatabaseInitializer para v2.0`
   - Click **Commit changes**

3. **Actualizar README.md:**
   - En la raíz del repositorio, click en `README.md`
   - Click en el **lápiz** (Edit)
   - Borra todo y pega el contenido del README.md actualizado
   - Mensaje de commit: `Update: README para v2.0`
   - Click **Commit changes**

4. **Agregar nuevos archivos:**
   - En la raíz del repositorio, click **Add file** → **Create new file**
   - Para `SOLUCION.md`:
     - Nombre: `SOLUCION.md`
     - Pega el contenido del archivo
     - Commit: `Add: Documentación de solución v2.0`
   
   - Repite para:
     - `GUIA-PUBLICACION.md`
     - `publish-windows.ps1`

#### Opción B: Subir todo de una vez (Más rápido)

1. Ve a: https://github.com/28Sibarita/MonkeyArtInventory
2. Click **Add file** → **Upload files**
3. Arrastra TODOS los archivos del proyecto (excepto carpetas bin, obj, publish-*)
4. Mensaje: `Release v2.0 - Corrección crítica de base de datos`
5. Click **Commit changes**

---

### PASO 2: Crear la Release v2.0

1. **Ve a Releases:**
   ```
   https://github.com/28Sibarita/MonkeyArtInventory/releases
   ```

2. **Crear nueva release:**
   - Click **Draft a new release**

3. **Configurar la release:**

   **Tag:**
   ```
   v2.0
   ```

   **Title:**
   ```
   MonkeyArtInventory v2.0 - Corrección Crítica
   ```

   **Descripción (copia y pega esto):**

```markdown
# 🎨 MonkeyArtInventory v2.0

## 🎉 Novedades

- 🐛 **Corrección crítica**: Error de inicialización de base de datos resuelto
- ✨ **Ejecutable portable**: No requiere instalación
- 🚀 **Auto-contenido**: No necesita .NET instalado
- 📖 **Documentación completa**: Guías y soluciones incluidas

## 📥 Descarga

### Windows (Recomendado)

**Descarga el archivo de abajo:** `MonkeyArtInventory.App.exe` o `MonkeyArtInventory-v2.0-Windows-Portable.zip`

### Cómo usar:
1. Descarga el archivo
2. Si descargaste el ZIP, descomprímelo
3. Haz doble clic en `MonkeyArtInventory.App.exe`
4. ¡Listo! La aplicación iniciará automáticamente

### Características:
- ✅ No requiere instalación
- ✅ No requiere .NET
- ✅ Ejecutable único de 93 MB
- ✅ Windows 10+ (64-bit)

## 🐛 Problema Resuelto

### Error anterior:
```
SQLite Error 1: 'no such table: Products'
```

### Solución:
Corrección en `DatabaseInitializer.cs` que mejora la lógica de inicialización de base de datos y manejo de migraciones.

Ver detalles en [SOLUCION.md](https://github.com/28Sibarita/MonkeyArtInventory/blob/main/SOLUCION.md)

## 📍 Datos de la Aplicación

Los datos se guardan en:
```
C:\Users\[TuUsuario]\AppData\Local\MonkeyArtInventory\
```

## 🛠️ Características Principales

- 📦 Gestión completa de inventario
- 📊 Reportes detallados en PDF/Excel
- 👥 Gestión de clientes
- 💰 Control de precios y consignación
- 📧 Envío automático de reportes
- ⏰ Programación de reportes semanales
- 💾 Respaldos automáticos

## 📞 Soporte

¿Problemas? Abre un [Issue](https://github.com/28Sibarita/MonkeyArtInventory/issues)

---

**Versión:** 2.0  
**Fecha:** 9 de enero de 2026  
**Plataforma:** Windows x64  
**Build:** Release
```

4. **Subir archivos:**
   - En la sección **"Attach binaries"** (al final)
   - Arrastra o selecciona:
     - `releases\MonkeyArtInventory-v2.0-Windows-Portable.zip` (87 MB)
     - O: `publish-windows-v2.0\MonkeyArtInventory.App.exe` (93 MB)
   
   💡 **IMPORTANTE**: GitHub permite archivos de hasta 100 MB, así que ambos archivos están OK.

5. **Opciones finales:**
   - ✅ Marca **"Set as the latest release"**
   - ✅ Marca **"Create a discussion for this release"** (opcional)

6. **Publicar:**
   - Click **"Publish release"** (botón verde)

---

### PASO 3: Verificar

1. **Ve a:**
   ```
   https://github.com/28Sibarita/MonkeyArtInventory/releases
   ```

2. **Deberías ver:**
   - ✅ Release v2.0 con el título y descripción
   - ✅ El archivo ejecutable disponible para descargar
   - ✅ Badge de "Latest" en verde

3. **Prueba el enlace de descarga:**
   - Click en el archivo
   - Debería descargar correctamente

---

## ✅ CHECKLIST FINAL

Antes de publicar, verifica:

- [ ] Código fuente actualizado en GitHub (DatabaseInitializer.cs y README.md)
- [ ] Tag v2.0 creado
- [ ] Release v2.0 publicada
- [ ] Ejecutable subido correctamente (93 MB o ZIP de 87 MB)
- [ ] Descripción completa con instrucciones
- [ ] Marcada como "Latest release"
- [ ] Enlace de descarga funciona

---

## 🎯 RESUMEN ULTRA RÁPIDO

1. **Actualiza código:**
   - https://github.com/28Sibarita/MonkeyArtInventory
   - Upload files → Arrastra todo → Commit

2. **Crea Release:**
   - https://github.com/28Sibarita/MonkeyArtInventory/releases/new
   - Tag: `v2.0`
   - Título: `MonkeyArtInventory v2.0 - Corrección Crítica`
   - Pega la descripción de arriba
   - Sube: `releases\MonkeyArtInventory-v2.0-Windows-Portable.zip`
   - Publish release

3. **¡Listo!** 🎉
   - Los usuarios podrán descargar desde:
   - https://github.com/28Sibarita/MonkeyArtInventory/releases

---

## 📞 ¿Necesitas Ayuda?

Si tienes problemas:
1. Verifica que estés logueado en GitHub
2. Asegúrate de tener permisos en el repositorio
3. Revisa que los archivos no excedan 100 MB
4. Intenta con el archivo ZIP (más pequeño)

---

**TODO ESTÁ LISTO - Solo sigue los pasos de arriba! 🚀**

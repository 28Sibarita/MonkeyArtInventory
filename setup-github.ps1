# Script para configurar Git y subir a GitHub
# MonkeyArtInventory v2.0

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  GitHub Setup - MonkeyArtInventory v2.0    " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# Verificar si Git está instalado
$gitInstalled = Get-Command git -ErrorAction SilentlyContinue

if (-not $gitInstalled) {
    Write-Host "Git no está instalado en este sistema" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Opciones:" -ForegroundColor Cyan
    Write-Host "  1. Instalar Git desde: https://git-scm.com/download/win" -ForegroundColor White
    Write-Host "  2. Usar GitHub Desktop: https://desktop.github.com/" -ForegroundColor White
    Write-Host "  3. Subir manualmente a GitHub usando la interfaz web" -ForegroundColor White
    Write-Host ""
    
    $choice = Read-Host "¿Deseas ver las instrucciones para subir manualmente? (S/N)"
    
    if ($choice -eq "S" -or $choice -eq "s") {
        Write-Host ""
        Write-Host "=== INSTRUCCIONES PARA SUBIR MANUALMENTE ===" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "1. Ve a GitHub.com y crea un nuevo repositorio o ve al repositorio existente:" -ForegroundColor Yellow
        Write-Host "   https://github.com/28Sibarita/MonkeyArtInventory" -ForegroundColor White
        Write-Host ""
        Write-Host "2. Crea una nueva release (versión):" -ForegroundColor Yellow
        Write-Host "   - Click en 'Releases' en el menú derecho" -ForegroundColor White
        Write-Host "   - Click en 'Draft a new release'" -ForegroundColor White
        Write-Host "   - Tag: v2.0" -ForegroundColor White
        Write-Host "   - Title: MonkeyArtInventory v2.0" -ForegroundColor White
        Write-Host ""
        Write-Host "3. Sube los siguientes archivos como assets:" -ForegroundColor Yellow
        Write-Host "   - publish-windows-v2.0\MonkeyArtInventory.App.exe (portable)" -ForegroundColor White
        Write-Host "   - installers\MonkeyArtInventory-Setup-v2.0.exe (instalador)" -ForegroundColor White
        Write-Host ""
        Write-Host "4. Escribe las notas de la versión (release notes)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "5. Publica la release" -ForegroundColor Yellow
        Write-Host ""
    }
    
    exit 0
}

Write-Host "✓ Git está instalado" -ForegroundColor Green
Write-Host ""

# Configuración del repositorio
$repoUrl = "https://github.com/28Sibarita/MonkeyArtInventory.git"
$version = "2.0"

# Verificar si ya es un repositorio Git
if (-not (Test-Path ".git")) {
    Write-Host "Inicializando repositorio Git..." -ForegroundColor Yellow
    git init
    
    Write-Host "Configurando remote origin..." -ForegroundColor Yellow
    git remote add origin $repoUrl
} else {
    Write-Host "✓ Repositorio Git ya inicializado" -ForegroundColor Green
    
    # Verificar remote
    $currentRemote = git remote get-url origin 2>$null
    if ($currentRemote -ne $repoUrl) {
        Write-Host "Actualizando remote origin..." -ForegroundColor Yellow
        git remote set-url origin $repoUrl
    }
}

Write-Host ""
Write-Host "=== Configuración de Git ===" -ForegroundColor Cyan
Write-Host ""

# Pedir nombre y email si no están configurados
$gitName = git config user.name 2>$null
$gitEmail = git config user.email 2>$null

if (-not $gitName) {
    $gitName = Read-Host "Ingresa tu nombre para Git"
    git config user.name "$gitName"
}

if (-not $gitEmail) {
    $gitEmail = Read-Host "Ingresa tu email para Git"
    git config user.email "$gitEmail"
}

Write-Host "✓ Usuario configurado: $gitName <$gitEmail>" -ForegroundColor Green
Write-Host ""

# Crear .gitignore si no existe
if (-not (Test-Path ".gitignore")) {
    Write-Host "Creando .gitignore..." -ForegroundColor Yellow
    
    $gitignoreContent = @"
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio cache/options
.vs/
.vscode/

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# Build outputs
publish/
publish-*/
installers/

# Database files
*.db
*.db-shm
*.db-wal

# OS files
.DS_Store
Thumbs.db

# Rider
.idea/
*.sln.iml
"@
    
    $gitignoreContent | Out-File -FilePath ".gitignore" -Encoding UTF8
}

Write-Host ""
Write-Host "=== Preparando commit ===" -ForegroundColor Cyan
Write-Host ""

# Mostrar estado
Write-Host "Estado actual del repositorio:" -ForegroundColor Yellow
git status --short

Write-Host ""
$confirm = Read-Host "¿Deseas hacer commit de estos cambios? (S/N)"

if ($confirm -eq "S" -or $confirm -eq "s") {
    # Agregar archivos
    Write-Host ""
    Write-Host "Agregando archivos..." -ForegroundColor Yellow
    git add .
    
    # Hacer commit
    $commitMessage = "Release v$version - Corrección de DatabaseInitializer y mejoras

- Corrección del bug en DatabaseInitializer.cs que impedía la creación de la BD
- Actualización a versión v$version
- Scripts de publicación mejorados
- Documentación actualizada
- Instalador Windows incluido"
    
    Write-Host "Creando commit..." -ForegroundColor Yellow
    git commit -m $commitMessage
    
    # Crear tag
    Write-Host "Creando tag v$version..." -ForegroundColor Yellow
    git tag -a "v$version" -m "Version $version"
    
    Write-Host ""
    Write-Host "✓ Commit y tag creados exitosamente" -ForegroundColor Green
    Write-Host ""
    
    # Preguntar si desea hacer push
    Write-Host "Para subir los cambios a GitHub, ejecuta:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  git push -u origin main" -ForegroundColor White
    Write-Host "  git push origin v$version" -ForegroundColor White
    Write-Host ""
    
    $pushNow = Read-Host "¿Deseas hacer push ahora? (S/N)"
    
    if ($pushNow -eq "S" -or $pushNow -eq "s") {
        Write-Host ""
        Write-Host "Subiendo cambios a GitHub..." -ForegroundColor Yellow
        
        # Intentar push
        git push -u origin main 2>&1
        git push origin "v$version" 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✓ Código subido exitosamente a GitHub!" -ForegroundColor Green
            Write-Host ""
            Write-Host "Siguiente paso:" -ForegroundColor Cyan
            Write-Host "  1. Ve a: https://github.com/28Sibarita/MonkeyArtInventory/releases" -ForegroundColor White
            Write-Host "  2. Crea una nueva release desde el tag v$version" -ForegroundColor White
            Write-Host "  3. Sube los instaladores como assets" -ForegroundColor White
        } else {
            Write-Host ""
            Write-Host "NOTA: El push falló. Puede ser porque:" -ForegroundColor Yellow
            Write-Host "  - Necesitas autenticarte (usa GitHub Desktop o token)" -ForegroundColor White
            Write-Host "  - El repositorio no existe en GitHub" -ForegroundColor White
            Write-Host "  - No tienes permisos" -ForegroundColor White
        }
    }
} else {
    Write-Host ""
    Write-Host "Commit cancelado" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Script completado" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

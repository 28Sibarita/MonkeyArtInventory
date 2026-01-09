; MonkeyArt Inventory - Inno Setup Script
; Version 2.0

#define MyAppName "MonkeyArt Inventario"
#define MyAppVersion "2.0"
#define MyAppPublisher "MonkeyArt"
#define MyAppURL "https://github.com/28Sibarita/MonkeyArtInventory"
#define MyAppExeName "MonkeyArtInventory.App.exe"

[Setup]
; Identificador único de la app (NO cambiar después de publicar)
AppId={{8F3C4A2B-5D1E-4F9A-B3C7-2A8E6D4F1B9C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={localappdata}\MonkeyArtInventory\App
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Iconos
SetupIconFile=..\publish\MonkeyArtInventory\app.ico
UninstallDisplayIcon={app}\app.ico
; Salida
OutputDir=..\installers
OutputBaseFilename=MonkeyArtInventory-Setup-v{#MyAppVersion}
; Compresión máxima
Compression=lzma2/ultra64
SolidCompression=yes
; Apariencia
WizardStyle=modern
; No requiere admin (instala en AppData del usuario)
PrivilegesRequired=lowest
; Información de desinstalación
UninstallDisplayName={#MyAppName}
; Permite actualizar sin desinstalar
UsePreviousAppDir=yes

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Copiar todos los archivos de la app
Source: "..\publish-windows-v2.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Icono
Source: "..\publish\MonkeyArtInventory\app.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Acceso directo en escritorio
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\app.ico"; Tasks: desktopicon
; Acceso directo en menú inicio
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\app.ico"

[Run]
; Ejecutar la app después de instalar (opcional)
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// Mostrar mensaje de bienvenida personalizado
function InitializeSetup(): Boolean;
begin
  Result := True;
end;

// Verificar si hay una versión anterior y ofrecer actualizar
function InitializeUninstall(): Boolean;
begin
  Result := True;
end;

[Messages]
spanish.WelcomeLabel1=Bienvenido al asistente de instalación de MonkeyArt Inventario
spanish.WelcomeLabel2=Este programa instalará MonkeyArt Inventario en su computadora.%n%nSe recomienda cerrar todas las demás aplicaciones antes de continuar.%n%nSus datos existentes NO serán eliminados.
spanish.FinishedLabel=La instalación de MonkeyArt Inventario ha finalizado.%n%nPuede ejecutar la aplicación haciendo clic en el icono del escritorio.

[UninstallDelete]
; NO borrar datos del usuario al desinstalar
; Los datos están en {localappdata}\MonkeyArtInventory\Data
Type: filesandordirs; Name: "{app}"

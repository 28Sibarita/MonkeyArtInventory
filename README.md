# MonkeyArtInventory

Offline inventory app for MonkeyArt (Dufer). WPF + .NET 8 + SQLite + EF Core.

## Requirements
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

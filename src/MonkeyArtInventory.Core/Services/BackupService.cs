using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Services;

public class BackupService
{
    private readonly SettingsService _settingsService;

    public BackupService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public string CreateBackup()
    {
        var settings = _settingsService.Current;
        var backupDir = _settingsService.GetBackupsDirectory();
        Directory.CreateDirectory(backupDir);

        var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupPath = Path.Combine(backupDir, $"inventory_{stamp}.db");

        File.Copy(settings.DatabasePath, backupPath, true);
        CleanupOldBackups(backupDir, settings.BackupRetention);

        return backupPath;
    }

    private static void CleanupOldBackups(string backupDir, int retention)
    {
        if (retention <= 0)
        {
            return;
        }

        var files = new DirectoryInfo(backupDir)
            .GetFiles("inventory_*.db")
            .OrderByDescending(f => f.CreationTimeUtc)
            .ToList();

        foreach (var file in files.Skip(retention))
        {
            file.Delete();
        }
    }
}

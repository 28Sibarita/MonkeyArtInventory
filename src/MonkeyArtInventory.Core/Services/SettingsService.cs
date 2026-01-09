using System.Text.Json;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Services;

public class SettingsService
{
    private readonly string _settingsPath;

    public SettingsService()
    {
        var baseDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MonkeyArtInventory");
        _settingsPath = Path.Combine(baseDir, "settings.json");
    }

    public AppSettings Current { get; private set; } = AppSettings.CreateDefault();

    public void Load()
    {
        if (!File.Exists(_settingsPath))
        {
            Current = AppSettings.CreateDefault();
            Save();
            return;
        }

        var json = File.ReadAllText(_settingsPath);
        var settings = JsonSerializer.Deserialize<AppSettings>(json);
        Current = settings ?? AppSettings.CreateDefault();
        EnsureDirectories();
    }

    public void Save()
    {
        EnsureDirectories();
        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
    }

    public void EnsureDirectories()
    {
        Directory.CreateDirectory(Current.DataDirectory);
        Directory.CreateDirectory(Current.ReportsDirectory);
        Directory.CreateDirectory(Current.LogsDirectory);
        Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Current.DatabasePath) ?? Current.DataDirectory));
        Directory.CreateDirectory(Path.Combine(GetBaseDirectory(), "Backups"));
    }

    public string GetBackupsDirectory()
    {
        return Path.Combine(GetBaseDirectory(), "Backups");
    }

    private static string GetBaseDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MonkeyArtInventory");
    }
}

namespace MonkeyArtInventory.Core.Models;

public class AppSettings
{
    public string DataDirectory { get; set; } = string.Empty;
    public string DatabasePath { get; set; } = string.Empty;
    public string ReportsDirectory { get; set; } = string.Empty;
    public string LogsDirectory { get; set; } = string.Empty;
    public string Currency { get; set; } = "COP";
    public bool EnablePrices { get; set; } = true;
    public int BackupRetention { get; set; } = 20;

    // Email Settings
    public bool EmailEnabled { get; set; } = false;
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string EmailFrom { get; set; } = string.Empty;
    public string EmailTo { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;

    public static AppSettings CreateDefault()
    {
        var baseDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MonkeyArtInventory");

        return new AppSettings
        {
            DataDirectory = Path.Combine(baseDir, "Data"),
            DatabasePath = Path.Combine(baseDir, "Data", "inventory.db"),
            ReportsDirectory = Path.Combine(baseDir, "Reportes"),
            LogsDirectory = Path.Combine(baseDir, "Logs"),
            Currency = "COP",
            EnablePrices = true,
            BackupRetention = 20,
            // Email defaults
            EmailEnabled = false,
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            SmtpUsername = string.Empty,
            SmtpPassword = string.Empty,
            EmailFrom = string.Empty,
            EmailTo = string.Empty,
            UseSsl = true
        };
    }
}

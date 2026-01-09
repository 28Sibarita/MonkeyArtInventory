using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.App.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly BackupService _backupService;
    private readonly EmailService _emailService;

    private string _dataDirectory = string.Empty;
    private string _databasePath = string.Empty;
    private string _reportsDirectory = string.Empty;
    private bool _enablePrices;
    private int _backupRetention;
    private string _currency = "COP";

    // Email fields
    private bool _emailEnabled;
    private string _smtpServer = "smtp.gmail.com";
    private int _smtpPort = 587;
    private string _smtpUsername = string.Empty;
    private string _smtpPassword = string.Empty;
    private string _emailFrom = string.Empty;
    private string _emailTo = string.Empty;
    private bool _useSsl = true;
    private bool _isSendingTestEmail;

    public SettingsViewModel(SettingsService settingsService, BackupService backupService, EmailService emailService)
    {
        _settingsService = settingsService;
        _backupService = backupService;
        _emailService = emailService;

        LoadFromSettings();

        SaveCommand = new RelayCommand(Save);
        CreateBackupCommand = new RelayCommand(CreateBackup);
        SendTestEmailCommand = new AsyncRelayCommand(SendTestEmailAsync);
    }

    public string DataDirectory
    {
        get => _dataDirectory;
        set => SetProperty(ref _dataDirectory, value);
    }

    public string DatabasePath
    {
        get => _databasePath;
        set => SetProperty(ref _databasePath, value);
    }

    public string ReportsDirectory
    {
        get => _reportsDirectory;
        set => SetProperty(ref _reportsDirectory, value);
    }

    public bool EnablePrices
    {
        get => _enablePrices;
        set => SetProperty(ref _enablePrices, value);
    }

    public int BackupRetention
    {
        get => _backupRetention;
        set => SetProperty(ref _backupRetention, value);
    }

    public string Currency
    {
        get => _currency;
        set => SetProperty(ref _currency, value);
    }

    // Email Properties
    public bool EmailEnabled
    {
        get => _emailEnabled;
        set => SetProperty(ref _emailEnabled, value);
    }

    public string SmtpServer
    {
        get => _smtpServer;
        set => SetProperty(ref _smtpServer, value);
    }

    public int SmtpPort
    {
        get => _smtpPort;
        set => SetProperty(ref _smtpPort, value);
    }

    public string SmtpUsername
    {
        get => _smtpUsername;
        set => SetProperty(ref _smtpUsername, value);
    }

    public string SmtpPassword
    {
        get => _smtpPassword;
        set => SetProperty(ref _smtpPassword, value);
    }

    public string EmailFrom
    {
        get => _emailFrom;
        set => SetProperty(ref _emailFrom, value);
    }

    public string EmailTo
    {
        get => _emailTo;
        set => SetProperty(ref _emailTo, value);
    }

    public bool UseSsl
    {
        get => _useSsl;
        set => SetProperty(ref _useSsl, value);
    }

    public bool IsSendingTestEmail
    {
        get => _isSendingTestEmail;
        set => SetProperty(ref _isSendingTestEmail, value);
    }

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CreateBackupCommand { get; }
    public IAsyncRelayCommand SendTestEmailCommand { get; }

    private void LoadFromSettings()
    {
        var current = _settingsService.Current;
        DataDirectory = current.DataDirectory;
        DatabasePath = current.DatabasePath;
        ReportsDirectory = current.ReportsDirectory;
        EnablePrices = current.EnablePrices;
        BackupRetention = current.BackupRetention;
        Currency = current.Currency;

        // Email settings
        EmailEnabled = current.EmailEnabled;
        SmtpServer = current.SmtpServer;
        SmtpPort = current.SmtpPort;
        SmtpUsername = current.SmtpUsername;
        SmtpPassword = current.SmtpPassword;
        EmailFrom = current.EmailFrom;
        EmailTo = current.EmailTo;
        UseSsl = current.UseSsl;
    }

    private void Save()
    {
        var current = _settingsService.Current;
        current.DataDirectory = DataDirectory;
        current.DatabasePath = DatabasePath;
        current.ReportsDirectory = ReportsDirectory;
        current.EnablePrices = EnablePrices;
        current.BackupRetention = BackupRetention;
        current.Currency = Currency;

        // Email settings
        current.EmailEnabled = EmailEnabled;
        current.SmtpServer = SmtpServer;
        current.SmtpPort = SmtpPort;
        current.SmtpUsername = SmtpUsername;
        current.SmtpPassword = SmtpPassword;
        current.EmailFrom = EmailFrom;
        current.EmailTo = EmailTo;
        current.UseSsl = UseSsl;

        _settingsService.Save();
        MessageBox.Show("Configuración guardada correctamente.", "Configuración", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void CreateBackup()
    {
        try
        {
            var backupPath = _backupService.CreateBackup();
            MessageBox.Show($"Backup created at {backupPath}", "Backups", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Backup failed: {ex.Message}", "Backups", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task SendTestEmailAsync()
    {
        IsSendingTestEmail = true;
        try
        {
            // Save settings first
            Save();

            var (success, message) = await _emailService.SendTestEmailAsync();
            if (success)
            {
                MessageBox.Show(message, "Correo de prueba", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(message, "Error de correo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            IsSendingTestEmail = false;
        }
    }
}

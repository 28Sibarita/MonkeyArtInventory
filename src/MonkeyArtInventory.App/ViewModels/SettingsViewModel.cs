using System.Windows;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.App.ViewModels;

public class DayOption
{
    public DayOfWeek Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly BackupService _backupService;
    private readonly EmailService _emailService;
    private readonly SchedulerService _schedulerService;

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

    // Scheduler fields
    private bool _schedulerEnabled;
    private DayOption? _selectedDay;
    private int _schedulerHour = 8;
    private int _schedulerMinute = 0;
    private string _schedulerStatus = string.Empty;
    private bool _isSendingScheduledReport;

    public SettingsViewModel(SettingsService settingsService, BackupService backupService, EmailService emailService, SchedulerService schedulerService)
    {
        _settingsService = settingsService;
        _backupService = backupService;
        _emailService = emailService;
        _schedulerService = schedulerService;

        // Initialize day options
        DayOptions = new ObservableCollection<DayOption>
        {
            new() { Value = DayOfWeek.Monday, Label = "Lunes" },
            new() { Value = DayOfWeek.Tuesday, Label = "Martes" },
            new() { Value = DayOfWeek.Wednesday, Label = "Miércoles" },
            new() { Value = DayOfWeek.Thursday, Label = "Jueves" },
            new() { Value = DayOfWeek.Friday, Label = "Viernes" },
            new() { Value = DayOfWeek.Saturday, Label = "Sábado" },
            new() { Value = DayOfWeek.Sunday, Label = "Domingo" }
        };

        // Initialize hour options (0-23)
        HourOptions = new ObservableCollection<int>(Enumerable.Range(0, 24));
        MinuteOptions = new ObservableCollection<int>(Enumerable.Range(0, 60).Where(m => m % 5 == 0)); // 0, 5, 10, ... 55

        LoadFromSettings();
        UpdateSchedulerStatus();

        SaveCommand = new RelayCommand(Save);
        CreateBackupCommand = new RelayCommand(CreateBackup);
        SendTestEmailCommand = new AsyncRelayCommand(SendTestEmailAsync);
        SendScheduledReportNowCommand = new AsyncRelayCommand(SendScheduledReportNowAsync);
        SaveSchedulerCommand = new RelayCommand(SaveSchedulerSettingsWithMessage);
    }

    public ObservableCollection<DayOption> DayOptions { get; }
    public ObservableCollection<int> HourOptions { get; }
    public ObservableCollection<int> MinuteOptions { get; }

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

    // Scheduler Properties
    public bool SchedulerEnabled
    {
        get => _schedulerEnabled;
        set
        {
            if (SetProperty(ref _schedulerEnabled, value))
            {
                UpdateSchedulerStatus();
            }
        }
    }

    public DayOption? SelectedDay
    {
        get => _selectedDay;
        set
        {
            if (SetProperty(ref _selectedDay, value))
            {
                UpdateSchedulerStatus();
            }
        }
    }

    public int SchedulerHour
    {
        get => _schedulerHour;
        set
        {
            if (SetProperty(ref _schedulerHour, value))
            {
                UpdateSchedulerStatus();
            }
        }
    }

    public int SchedulerMinute
    {
        get => _schedulerMinute;
        set
        {
            if (SetProperty(ref _schedulerMinute, value))
            {
                UpdateSchedulerStatus();
            }
        }
    }

    public string SchedulerStatus
    {
        get => _schedulerStatus;
        set => SetProperty(ref _schedulerStatus, value);
    }

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CreateBackupCommand { get; }
    public IAsyncRelayCommand SendTestEmailCommand { get; }
    public IAsyncRelayCommand SendScheduledReportNowCommand { get; }
    public IRelayCommand SaveSchedulerCommand { get; }

    public bool IsSendingScheduledReport
    {
        get => _isSendingScheduledReport;
        set => SetProperty(ref _isSendingScheduledReport, value);
    }

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

        // Scheduler settings
        SchedulerEnabled = current.SchedulerEnabled;
        SelectedDay = DayOptions.FirstOrDefault(d => d.Value == current.SchedulerDay) ?? DayOptions[0];
        SchedulerHour = current.SchedulerHour;
        SchedulerMinute = current.SchedulerMinute;
    }

    private void UpdateSchedulerStatus()
    {
        if (!SchedulerEnabled)
        {
            SchedulerStatus = "Envío automático desactivado";
            return;
        }

        if (!EmailEnabled)
        {
            SchedulerStatus = "⚠️ Primero habilita y configura el correo electrónico";
            return;
        }

        var status = _schedulerService.GetNextRunDescription();
        SchedulerStatus = $"📅 {status}";
    }

    private void SaveSchedulerSettings()
    {
        var current = _settingsService.Current;
        current.SchedulerEnabled = SchedulerEnabled;
        current.SchedulerDay = SelectedDay?.Value ?? DayOfWeek.Monday;
        current.SchedulerHour = SchedulerHour;
        current.SchedulerMinute = SchedulerMinute;
        _settingsService.Save();
        _schedulerService.Restart();
        UpdateSchedulerStatus();
    }

    private void SaveSchedulerSettingsWithMessage()
    {
        SaveSchedulerSettings();
        MessageBox.Show("Configuración de envío automático guardada.", "Configuración", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async Task SendScheduledReportNowAsync()
    {
        if (!EmailEnabled || !_emailService.IsConfigured())
        {
            MessageBox.Show("Primero configura el correo electrónico.", "Enviar informe", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsSendingScheduledReport = true;
        try
        {
            var (success, message) = await _schedulerService.SendNowAsync();
            if (success)
            {
                MessageBox.Show("Informe enviado correctamente.", "Enviar informe", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al enviar: {message}", "Enviar informe", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            IsSendingScheduledReport = false;
        }
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

        // Scheduler settings
        current.SchedulerEnabled = SchedulerEnabled;
        current.SchedulerDay = SelectedDay?.Value ?? DayOfWeek.Monday;
        current.SchedulerHour = SchedulerHour;
        current.SchedulerMinute = SchedulerMinute;

        _settingsService.Save();
        
        // Restart scheduler with new settings
        _schedulerService.Restart();
        
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

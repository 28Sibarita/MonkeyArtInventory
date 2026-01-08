using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.App.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly BackupService _backupService;

    private string _dataDirectory = string.Empty;
    private string _databasePath = string.Empty;
    private string _reportsDirectory = string.Empty;
    private bool _enablePrices;
    private int _backupRetention;
    private string _currency = "COP";

    public SettingsViewModel(SettingsService settingsService, BackupService backupService)
    {
        _settingsService = settingsService;
        _backupService = backupService;

        LoadFromSettings();

        SaveCommand = new RelayCommand(Save);
        CreateBackupCommand = new RelayCommand(CreateBackup);
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

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CreateBackupCommand { get; }

    private void LoadFromSettings()
    {
        var current = _settingsService.Current;
        DataDirectory = current.DataDirectory;
        DatabasePath = current.DatabasePath;
        ReportsDirectory = current.ReportsDirectory;
        EnablePrices = current.EnablePrices;
        BackupRetention = current.BackupRetention;
        Currency = current.Currency;
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

        _settingsService.Save();
        MessageBox.Show("Settings saved. Restart the app if paths changed.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
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
}

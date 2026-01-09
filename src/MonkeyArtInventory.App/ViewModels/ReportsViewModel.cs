using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.App.Infrastructure;
using System;
using System.IO;

namespace MonkeyArtInventory.App.ViewModels;

public class ReportsViewModel : ObservableObject
{
    private readonly ReportService _reportService;
    private readonly SettingsService _settingsService;
    private readonly EmailService _emailService;
    private readonly NavigationStore _navigationStore;
    private readonly Func<InformesDetalladoViewModel> _informesFactory;

    private WeeklyReportDto? _report;
    private bool _isBusy;
    private string _outputDirectory;
    private string? _emailStatus;
    private string? _lastExportedPdfPath;

    public ReportsViewModel(ReportService reportService, SettingsService settingsService, EmailService emailService, NavigationStore navigationStore, Func<InformesDetalladoViewModel> informesFactory)
    {
        _reportService = reportService;
        _settingsService = settingsService;
        _emailService = emailService;
        _navigationStore = navigationStore;
        _informesFactory = informesFactory;
        _outputDirectory = _settingsService.Current.ReportsDirectory;

        GenerateReportCommand = new AsyncRelayCommand(GenerateAsync);
        ExportReportCommand = new RelayCommand(Export, () => Report is not null);
        SendEmailCommand = new AsyncRelayCommand(SendEmailAsync);
        OpenDetailedCommand = new RelayCommand(OpenDetailed);

        // Load report asynchronously on UI thread (fire and forget)
        _ = LoadInitialReportAsync();
    }

    private async Task LoadInitialReportAsync()
    {
        try
        {
            await GenerateAsync();
        }
        catch
        {
            // Ignore startup errors
        }
    }

    public WeeklyReportDto? Report
    {
        get => _report;
        set
        {
            SetProperty(ref _report, value);
            ExportReportCommand.NotifyCanExecuteChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string OutputDirectory
    {
        get => _outputDirectory;
        set => SetProperty(ref _outputDirectory, value);
    }

    public string? EmailStatus
    {
        get => _emailStatus;
        set => SetProperty(ref _emailStatus, value);
    }

    public bool CanSendEmail
    {
        get
        {
            try
            {
                return _emailService?.IsConfigured() ?? false;
            }
            catch
            {
                return false;
            }
        }
    }

    public IAsyncRelayCommand GenerateReportCommand { get; }
    public IRelayCommand ExportReportCommand { get; }
    public IAsyncRelayCommand SendEmailCommand { get; }
    public IRelayCommand OpenDetailedCommand { get; }

    private async Task GenerateAsync()
    {
        IsBusy = true;
        try
        {
            Report = await _reportService.BuildWeeklyReportAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al generar informe: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Export()
    {
        if (Report is null)
        {
            return;
        }

        var output = string.IsNullOrWhiteSpace(OutputDirectory)
            ? _settingsService.Current.ReportsDirectory
            : OutputDirectory;

        // ExportWeeklyReport now returns the PDF path
        _lastExportedPdfPath = _reportService.ExportWeeklyReport(Report, output);
        
        MessageBox.Show("Informe exportado correctamente.", "Informes", MessageBoxButton.OK, MessageBoxImage.Information);
        OnPropertyChanged(nameof(CanSendEmail));
    }

    private async Task SendEmailAsync()
    {
        EmailStatus = null;

        // If no report exported yet, export first
        if (string.IsNullOrEmpty(_lastExportedPdfPath) || !File.Exists(_lastExportedPdfPath))
        {
            if (Report is null)
            {
                MessageBox.Show("Primero genera el informe.", "Enviar correo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Export the report first
            var output = string.IsNullOrWhiteSpace(OutputDirectory)
                ? _settingsService.Current.ReportsDirectory
                : OutputDirectory;

            _lastExportedPdfPath = _reportService.ExportWeeklyReport(Report, output);
        }

        IsBusy = true;
        try
        {
            var (success, message) = await _emailService.SendReportAsync(_lastExportedPdfPath);
            
            if (success)
            {
                EmailStatus = $"✓ {message}";
                MessageBox.Show(message, "Correo enviado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                EmailStatus = null;
                MessageBox.Show(message, "Error al enviar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OpenDetailed()
    {
        var vm = _informesFactory();
        _navigationStore.CurrentViewModel = vm;
        // trigger load asynchronously if available
        if (vm.LoadCommand is IAsyncRelayCommand asyncCmd && asyncCmd.CanExecute(null))
        {
            asyncCmd.Execute(null);
        }
    }
}

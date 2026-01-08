using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.App.Infrastructure;
using System;

namespace MonkeyArtInventory.App.ViewModels;

public class ReportsViewModel : ObservableObject
{
    private readonly ReportService _reportService;
    private readonly SettingsService _settingsService;
    private readonly NavigationStore _navigationStore;
    private readonly Func<InformesDetalladoViewModel> _informesFactory;

    private WeeklyReportDto? _report;
    private bool _isBusy;
    private string _outputDirectory;

    public ReportsViewModel(ReportService reportService, SettingsService settingsService, NavigationStore navigationStore, Func<InformesDetalladoViewModel> informesFactory)
    {
        _reportService = reportService;
        _settingsService = settingsService;
        _navigationStore = navigationStore;
        _informesFactory = informesFactory;
        _outputDirectory = _settingsService.Current.ReportsDirectory;

        GenerateReportCommand = new AsyncRelayCommand(GenerateAsync);
        ExportReportCommand = new RelayCommand(Export, () => Report is not null);
        OpenDetailedCommand = new RelayCommand(OpenDetailed);

        _ = GenerateAsync();
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

    public IAsyncRelayCommand GenerateReportCommand { get; }
    public IRelayCommand ExportReportCommand { get; }
    public IRelayCommand OpenDetailedCommand { get; }

    private async Task GenerateAsync()
    {
        IsBusy = true;
        try
        {
            Report = await _reportService.BuildWeeklyReportAsync();
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

        _reportService.ExportWeeklyReport(Report, output);
        MessageBox.Show("Report exported.", "Reports", MessageBoxButton.OK, MessageBoxImage.Information);
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

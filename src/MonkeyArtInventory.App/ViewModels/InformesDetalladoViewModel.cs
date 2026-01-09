using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Core.DTOs;

namespace MonkeyArtInventory.App.ViewModels;

public class InformesDetalladoViewModel : ObservableObject
{
    private readonly ReportService _reportService;
    private readonly SettingsService _settingsService;

    public ObservableCollection<BitmapImage> ChartImages { get; } = new();

    public IAsyncRelayCommand LoadCommand { get; }

    public InformesDetalladoViewModel(ReportService reportService, SettingsService settingsService)
    {
        _reportService = reportService;
        _settingsService = settingsService;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
    }

    private async Task LoadAsync()
    {
        ChartImages.Clear();
        var report = await _reportService.BuildWeeklyReportAsync();
        var outputDir = _settingsService.Current.ReportsDirectory;
        var paths = _reportService.GenerateChartImages(report, outputDir);

        foreach (var p in paths)
        {
            try
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = new Uri(p);
                img.EndInit();
                ChartImages.Add(img);
            }
            catch
            {
                // ignore load failures
            }
        }
    }
}

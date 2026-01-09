using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.App.Infrastructure;
using MonkeyArtInventory.App.ViewModels;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Data;
using MonkeyArtInventory.Data.Services;

namespace MonkeyArtInventory.App;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var log = Path.Combine(Path.GetTempPath(), "MonkeyArtInventory_app_start_error.log");

            // Global exception handlers to capture unhandled exceptions during startup
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                try { File.AppendAllText(log, ex.ExceptionObject?.ToString() + Environment.NewLine); } catch { }
            };

            TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                try { File.AppendAllText(log, ex.Exception.ToString() + Environment.NewLine); } catch { }
            };

            this.DispatcherUnhandledException += (s, ex) =>
            {
                try { File.AppendAllText(log, ex.Exception.ToString() + Environment.NewLine); } catch { }
                ex.Handled = false;
            };

            var services = new ServiceCollection();
        var settingsService = new SettingsService();
        settingsService.Load();

        services.AddSingleton(settingsService);
        services.AddSingleton<BackupService>();
        services.AddSingleton<EmailService>();

        services.AddDbContext<InventoryDbContext>(options =>
        {
            options.UseSqlite($"Data Source={settingsService.Current.DatabasePath}");
        });

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ProductService>();
        services.AddScoped<MovementService>();
        services.AddScoped<ReportService>();
        services.AddScoped<ClientService>();

        services.AddSingleton<NavigationStore>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<ProductDetailViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<InformesDetalladoViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ClientsViewModel>();
        services.AddTransient<Func<DashboardViewModel>>(sp => () => sp.GetRequiredService<DashboardViewModel>());
        services.AddTransient<Func<InventoryViewModel>>(sp => () => sp.GetRequiredService<InventoryViewModel>());
        services.AddTransient<Func<ProductDetailViewModel>>(sp => () => sp.GetRequiredService<ProductDetailViewModel>());
        services.AddTransient<Func<ReportsViewModel>>(sp => () => sp.GetRequiredService<ReportsViewModel>());
        services.AddTransient<Func<InformesDetalladoViewModel>>(sp => () => sp.GetRequiredService<InformesDetalladoViewModel>());
        services.AddTransient<Func<SettingsViewModel>>(sp => () => sp.GetRequiredService<SettingsViewModel>());
        services.AddTransient<Func<ClientsViewModel>>(sp => () => sp.GetRequiredService<ClientsViewModel>());
        services.AddTransient<Func<MovementType, MovementEntryViewModel>>(sp =>
        {
            return type => new MovementEntryViewModel(
                type,
                sp.GetRequiredService<ProductService>(),
                sp.GetRequiredService<MovementService>(),
                sp.GetRequiredService<SettingsService>(),
                sp.GetRequiredService<ClientService>());
        });

        _serviceProvider = services.BuildServiceProvider();

        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            await DatabaseInitializer.InitializeAsync(db);
        }

        if (IsCliMode(e.Args, out var outputPath))
        {
            await RunWeeklyReportAsync(outputPath, settingsService);
            Shutdown();
            return;
        }

            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }
        catch (Exception ex)
        {
            try
            {
                var log = Path.Combine(Path.GetTempPath(), "MonkeyArtInventory_app_start_error.log");
                File.WriteAllText(log, ex.ToString());
            }
            catch
            {
                // ignore
            }

            Environment.Exit(1);
        }
    }

    private bool IsCliMode(string[] args, out string? outputPath)
    {
        outputPath = null;
        var isWeekly = args.Any(a => string.Equals(a, "--weekly-report", StringComparison.OrdinalIgnoreCase)
            || string.Equals(a, "--export-weekly-report", StringComparison.OrdinalIgnoreCase));

        if (!isWeekly)
        {
            return false;
        }

        var outputIndex = Array.FindIndex(args, a => string.Equals(a, "--report-output", StringComparison.OrdinalIgnoreCase));
        if (outputIndex >= 0 && outputIndex < args.Length - 1)
        {
            outputPath = args[outputIndex + 1];
        }

        return true;
    }

    private async Task RunWeeklyReportAsync(string? outputPath, SettingsService settingsService)
    {
        var logPath = Path.Combine(settingsService.Current.LogsDirectory, "weekly_report.log");
        Directory.CreateDirectory(settingsService.Current.LogsDirectory);

        try
        {
            using var scope = _serviceProvider!.CreateScope();
            var reportService = scope.ServiceProvider.GetRequiredService<ReportService>();
            var report = await reportService.BuildWeeklyReportAsync();

            var targetDirectory = string.IsNullOrWhiteSpace(outputPath)
                ? settingsService.Current.ReportsDirectory
                : outputPath;

            reportService.ExportWeeklyReport(report, targetDirectory);
            File.AppendAllText(logPath, $"{DateTime.Now:u} Weekly report generated at {targetDirectory}{Environment.NewLine}");
            Environment.ExitCode = 0;
        }
        catch (Exception ex)
        {
            File.AppendAllText(logPath, $"{DateTime.Now:u} Weekly report failed: {ex.Message}{Environment.NewLine}");
            Environment.ExitCode = 1;
        }
    }
}

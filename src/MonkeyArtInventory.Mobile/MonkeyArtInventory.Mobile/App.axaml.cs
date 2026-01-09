using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System;
using System.IO;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Data;
using MonkeyArtInventory.Data.Services;
using MonkeyArtInventory.Mobile.ViewModels;
using MonkeyArtInventory.Mobile.Views;

namespace MonkeyArtInventory.Mobile;

public partial class App : Application
{
    public static ServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            // Configure services
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            // Initialize database
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                    DatabaseInitializer.InitializeAsync(db).Wait();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database init error: {ex.Message}");
            }

            var mainViewModel = Services.GetRequiredService<MainViewModel>();
            
            // Inject services into MainViewModel
            mainViewModel.ProductService = Services.GetRequiredService<ProductService>();
            mainViewModel.MovementService = Services.GetRequiredService<MovementService>();
            mainViewModel.ClientService = Services.GetRequiredService<ClientService>();
            mainViewModel.SettingsService = Services.GetRequiredService<SettingsService>();
            
            // Initialize with services
            mainViewModel.Initialize();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainViewModel
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"App initialization error: {ex.Message}");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        var settingsService = new SettingsService();
        
        try
        {
            settingsService.Load();
        }
        catch
        {
            // Ignore settings load errors on first run
        }

        // For Android/Mobile, use a simpler path approach
        string dbPath;
        try
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = ".";
            }
            
            var dbDir = Path.Combine(basePath, "MonkeyArtInventory");
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            dbPath = Path.Combine(dbDir, "inventory.db");
        }
        catch
        {
            // Fallback to current directory
            dbPath = "inventory.db";
        }

        services.AddSingleton(settingsService);
        services.AddSingleton<BackupService>();

        services.AddDbContext<InventoryDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ProductService>();
        services.AddScoped<MovementService>();
        services.AddScoped<ReportService>();
        services.AddScoped<ClientService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<ClientsViewModel>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
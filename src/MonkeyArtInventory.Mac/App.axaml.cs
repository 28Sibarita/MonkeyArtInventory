using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Data;
using MonkeyArtInventory.Data.Services;
using MonkeyArtInventory.Mac.ViewModels;
using MonkeyArtInventory.Mac.Views;

namespace MonkeyArtInventory.Mac;

public partial class App : Application
{
    public static ServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            // Configure services
            var services = new ServiceCollection();
            var settingsService = new SettingsService();
            settingsService.Load();

            services.AddSingleton(settingsService);
            services.AddSingleton<BackupService>();

            services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseSqlite($"Data Source={settingsService.Current.DatabasePath}");
            });

            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<ProductService>();
            services.AddScoped<MovementService>();
            services.AddScoped<ReportService>();
            services.AddScoped<ClientService>();

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<InventoryViewModel>();
            services.AddTransient<ProductDetailViewModel>();
            services.AddTransient<ClientsViewModel>();

            Services = services.BuildServiceProvider();

            // Initialize database
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                DatabaseInitializer.InitializeAsync(db).Wait();
            }

            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Mac.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private string _currentViewName = "Dashboard";

    public MainWindowViewModel()
    {
        _currentView = new DashboardViewModel();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentView = new DashboardViewModel();
        CurrentViewName = "Dashboard";
    }

    [RelayCommand]
    private void NavigateToInventory()
    {
        CurrentView = new InventoryViewModel();
        CurrentViewName = "Inventario";
    }

    [RelayCommand]
    private void NavigateToClients()
    {
        if (App.Services != null)
        {
            CurrentView = App.Services.GetRequiredService<ClientsViewModel>();
            CurrentViewName = "Clientes";
        }
    }

    [RelayCommand]
    private void NavigateToEntry()
    {
        if (App.Services != null)
        {
            var productService = App.Services.GetRequiredService<ProductService>();
            var movementService = App.Services.GetRequiredService<MovementService>();
            var settingsService = App.Services.GetRequiredService<SettingsService>();
            var clientService = App.Services.GetRequiredService<ClientService>();
            CurrentView = new MovementEntryViewModel(MovementType.Entrada, productService, movementService, settingsService, clientService);
            CurrentViewName = "Entrada";
        }
    }

    [RelayCommand]
    private void NavigateToExit()
    {
        if (App.Services != null)
        {
            var productService = App.Services.GetRequiredService<ProductService>();
            var movementService = App.Services.GetRequiredService<MovementService>();
            var settingsService = App.Services.GetRequiredService<SettingsService>();
            var clientService = App.Services.GetRequiredService<ClientService>();
            CurrentView = new MovementEntryViewModel(MovementType.Salida, productService, movementService, settingsService, clientService);
            CurrentViewName = "Salida";
        }
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentViewName = "Configuración";
    }
}

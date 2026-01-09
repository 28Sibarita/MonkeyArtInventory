using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Mobile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _currentView;

    [ObservableProperty]
    private string _currentViewName = "Dashboard";

    [ObservableProperty]
    private bool _isMenuOpen;

    // Services - to be injected
    public ProductService? ProductService { get; set; }
    public MovementService? MovementService { get; set; }
    public ClientService? ClientService { get; set; }
    public SettingsService? SettingsService { get; set; }

    private bool _isInitialized;

    public MainViewModel()
    {
        // Start with empty dashboard - will be replaced when services are injected
        CurrentView = new DashboardViewModel();
    }

    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        // Now that services are available, navigate to dashboard
        NavigateToDashboard();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        if (ProductService != null && MovementService != null)
        {
            CurrentView = new DashboardViewModel(ProductService, MovementService);
        }
        else
        {
            CurrentView = new DashboardViewModel();
        }
        CurrentViewName = "Dashboard";
        IsMenuOpen = false;
    }

    [RelayCommand]
    private void NavigateToInventory()
    {
        if (ProductService != null && MovementService != null)
        {
            CurrentView = new InventoryViewModel(ProductService, MovementService);
        }
        else
        {
            CurrentView = new InventoryViewModel();
        }
        CurrentViewName = "Inventario";
        IsMenuOpen = false;
    }

    [RelayCommand]
    private void NavigateToClients()
    {
        if (ClientService != null)
        {
            CurrentView = new ClientsViewModel(ClientService);
            CurrentViewName = "Clientes";
            IsMenuOpen = false;
        }
    }

    [RelayCommand]
    private void NavigateToEntry()
    {
        if (ProductService != null && MovementService != null && SettingsService != null && ClientService != null)
        {
            CurrentView = new MovementEntryViewModel(MovementType.Entrada, ProductService, MovementService, SettingsService, ClientService);
            CurrentViewName = "Entrada";
            IsMenuOpen = false;
        }
    }

    [RelayCommand]
    private void NavigateToExit()
    {
        if (ProductService != null && MovementService != null && SettingsService != null && ClientService != null)
        {
            CurrentView = new MovementEntryViewModel(MovementType.Salida, ProductService, MovementService, SettingsService, ClientService);
            CurrentViewName = "Salida";
            IsMenuOpen = false;
        }
    }

    [RelayCommand]
    private void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }
}

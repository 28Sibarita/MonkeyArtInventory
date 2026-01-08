using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.App.Infrastructure;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.App.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly NavigationStore _navigationStore;
    private readonly Func<DashboardViewModel> _dashboardFactory;
    private readonly Func<InventoryViewModel> _inventoryFactory;
    private readonly Func<ReportsViewModel> _reportsFactory;
    private readonly Func<SettingsViewModel> _settingsFactory;
    private readonly Func<MovementType, MovementEntryViewModel> _movementFactory;

    public MainViewModel(
        NavigationStore navigationStore,
        Func<DashboardViewModel> dashboardFactory,
        Func<InventoryViewModel> inventoryFactory,
        Func<ReportsViewModel> reportsFactory,
        Func<SettingsViewModel> settingsFactory,
        Func<MovementType, MovementEntryViewModel> movementFactory)
    {
        _navigationStore = navigationStore;
        _dashboardFactory = dashboardFactory;
        _inventoryFactory = inventoryFactory;
        _reportsFactory = reportsFactory;
        _settingsFactory = settingsFactory;
        _movementFactory = movementFactory;

        _navigationStore.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(NavigationStore.CurrentViewModel))
            {
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        };

        ShowDashboardCommand = new RelayCommand(ShowDashboard);
        ShowInventoryCommand = new RelayCommand(ShowInventory);
        ShowReportsCommand = new RelayCommand(ShowReports);
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        ShowEntryCommand = new RelayCommand(() => ShowMovement(MovementType.Entrada));
        ShowExitCommand = new RelayCommand(() => ShowMovement(MovementType.Salida));
        ShowAdjustmentCommand = new RelayCommand(() => ShowMovement(MovementType.Ajuste));
        ShowLossCommand = new RelayCommand(() => ShowMovement(MovementType.Merma));

        ShowDashboard();
    }

    public ObservableObject? CurrentViewModel => _navigationStore.CurrentViewModel;

    public IRelayCommand ShowDashboardCommand { get; }
    public IRelayCommand ShowInventoryCommand { get; }
    public IRelayCommand ShowReportsCommand { get; }
    public IRelayCommand ShowSettingsCommand { get; }
    public IRelayCommand ShowEntryCommand { get; }
    public IRelayCommand ShowExitCommand { get; }
    public IRelayCommand ShowAdjustmentCommand { get; }
    public IRelayCommand ShowLossCommand { get; }

    private void ShowDashboard()
    {
        _navigationStore.CurrentViewModel = _dashboardFactory();
    }

    private void ShowInventory()
    {
        _navigationStore.CurrentViewModel = _inventoryFactory();
    }

    private void ShowReports()
    {
        _navigationStore.CurrentViewModel = _reportsFactory();
    }

    private void ShowSettings()
    {
        _navigationStore.CurrentViewModel = _settingsFactory();
    }

    private void ShowMovement(MovementType type)
    {
        _navigationStore.CurrentViewModel = _movementFactory(type);
    }
}

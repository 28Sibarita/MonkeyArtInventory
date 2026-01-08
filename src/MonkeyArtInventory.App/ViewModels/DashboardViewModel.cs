using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.App.Infrastructure;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using System.Collections.ObjectModel;

namespace MonkeyArtInventory.App.ViewModels;

public class DashboardViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly MovementService _movementService;
    private readonly NavigationStore _navigationStore;
    private readonly Func<MovementType, MovementEntryViewModel> _movementFactory;
    private readonly Func<InventoryViewModel> _inventoryFactory;
    private readonly Func<ReportsViewModel> _reportsFactory;

    private int _totalProducts;
    private int _totalStock;
    private int _lowStockCount;
    private int _todayMovements;
    private ObservableCollection<string> _detailItems = new();

    public DashboardViewModel(
        ProductService productService,
        MovementService movementService,
        NavigationStore navigationStore,
        Func<MovementType, MovementEntryViewModel> movementFactory,
        Func<InventoryViewModel> inventoryFactory,
        Func<ReportsViewModel> reportsFactory)
    {
        _productService = productService;
        _movementService = movementService;
        _navigationStore = navigationStore;
        _movementFactory = movementFactory;
        _inventoryFactory = inventoryFactory;
        _reportsFactory = reportsFactory;

        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        RegisterEntryCommand = new RelayCommand(() => NavigateToMovement(MovementType.Entrada));
        RegisterExitCommand = new RelayCommand(() => NavigateToMovement(MovementType.Salida));
        ViewInventoryCommand = new RelayCommand(() => _navigationStore.CurrentViewModel = _inventoryFactory());
        ViewReportsCommand = new RelayCommand(() => _navigationStore.CurrentViewModel = _reportsFactory());

        _ = LoadAsync();
        ShowActiveProductsCommand = new RelayCommand(async () => await ShowActiveProductsAsync());
        ShowLowStockProductsCommand = new RelayCommand(async () => await ShowLowStockProductsAsync());
        ShowStockByProductsCommand = new RelayCommand(async () => await ShowStockByProductsAsync());
        ShowTodayMovementsProductsCommand = new RelayCommand(async () => await ShowTodayMovementsProductsAsync());
    }

    public int TotalProducts
    {
        get => _totalProducts;
        set => SetProperty(ref _totalProducts, value);
    }

    public int TotalStock
    {
        get => _totalStock;
        set => SetProperty(ref _totalStock, value);
    }

    public int LowStockCount
    {
        get => _lowStockCount;
        set => SetProperty(ref _lowStockCount, value);
    }

    public int TodayMovements
    {
        get => _todayMovements;
        set => SetProperty(ref _todayMovements, value);
    }

    public ObservableCollection<string> DetailItems
    {
        get => _detailItems;
        set => SetProperty(ref _detailItems, value);
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IRelayCommand RegisterEntryCommand { get; }
    public IRelayCommand RegisterExitCommand { get; }
    public IRelayCommand ViewInventoryCommand { get; }
    public IRelayCommand ViewReportsCommand { get; }
    public IRelayCommand ShowActiveProductsCommand { get; }
    public IRelayCommand ShowLowStockProductsCommand { get; }
    public IRelayCommand ShowStockByProductsCommand { get; }
    public IRelayCommand ShowTodayMovementsProductsCommand { get; }

    private async Task LoadAsync()
    {
        var list = await _productService.GetListAsync(new ProductListFilter(null, null, false, false));
        TotalProducts = list.Count(p => p.IsActive);
        TotalStock = list.Sum(p => p.Stock);
        LowStockCount = list.Count(p => p.Stock <= p.StockMinimo);
        TodayMovements = await _movementService.GetTodayMovementCountAsync();
    }

    private async Task ShowActiveProductsAsync()
    {
        var list = await _productService.GetListAsync(new ProductListFilter(null, null, false, false));
        var names = list.Where(p => p.IsActive).Select(p => p.Name).ToList();
        UpdateDetails(names);
    }

    private async Task ShowLowStockProductsAsync()
    {
        var list = await _productService.GetListAsync(new ProductListFilter(null, null, true, false));
        var names = list.Select(p => p.Name).ToList();
        UpdateDetails(names);
    }

    private async Task ShowStockByProductsAsync()
    {
        var list = await _productService.GetListAsync(new ProductListFilter(null, null, false, false));
        var ordered = list.OrderByDescending(p => p.Stock).Take(20).Select(p => $"{p.Name} — {p.Stock}").ToList();
        UpdateDetails(ordered);
    }

    private async Task ShowTodayMovementsProductsAsync()
    {
        var movements = await _movementService.GetTodayMovementsAsync();
        var products = new List<string>();
        foreach (var m in movements)
        {
            var product = await _productService.GetByIdAsync(m.ProductId);
            if (product != null)
            {
                products.Add($"{product.Name} — {m.Type} ({m.Quantity})");
            }
        }
        UpdateDetails(products);
    }

    private void UpdateDetails(IEnumerable<string> items)
    {
        DetailItems.Clear();
        foreach (var it in items)
            DetailItems.Add(it);
    }

    private void NavigateToMovement(MovementType type)
    {
        _navigationStore.CurrentViewModel = _movementFactory(type);
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Mobile.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ProductService? _productService;
    private readonly MovementService? _movementService;

    [ObservableProperty]
    private int _totalProducts;

    [ObservableProperty]
    private int _lowStockCount;

    [ObservableProperty]
    private int _todayMovements;

    [ObservableProperty]
    private ObservableCollection<ProductListItemDto> _lowStockProducts = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public DashboardViewModel()
    {
        // Design-time constructor
    }

    public DashboardViewModel(ProductService productService, MovementService movementService)
    {
        _productService = productService;
        _movementService = movementService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (_productService == null || _movementService == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var products = await _productService.GetListAsync(new ProductListFilter(null, null, false, false));
            TotalProducts = products.Count;

            var lowStock = products.Where(p => p.Stock <= p.StockMinimo).ToList();
            LowStockCount = lowStock.Count;
            LowStockProducts = new ObservableCollection<ProductListItemDto>(lowStock.Take(5));

            TodayMovements = 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"Dashboard load error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}

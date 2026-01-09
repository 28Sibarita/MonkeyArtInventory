using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MonkeyArtInventory.Mac.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ProductService _productService;
    private readonly MovementService _movementService;

    [ObservableProperty]
    private int _totalProductos;

    [ObservableProperty]
    private int _stockBajo;

    [ObservableProperty]
    private int _entradasHoy;

    [ObservableProperty]
    private int _salidasHoy;

    [ObservableProperty]
    private ObservableCollection<ProductListItemDto> _productos = new();

    public DashboardViewModel()
    {
        _productService = App.Services!.GetRequiredService<ProductService>();
        _movementService = App.Services!.GetRequiredService<MovementService>();
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var filter = new ProductListFilter(null, null, false, false);
        var products = await _productService.GetListAsync(filter);
        Productos = new ObservableCollection<ProductListItemDto>(products);
        
        TotalProductos = Productos.Count;
        StockBajo = 0;
        foreach (var p in Productos)
        {
            if (p.Stock <= p.StockMinimo) StockBajo++;
        }

        var todayMovements = await _movementService.GetTodayMovementsAsync();
        EntradasHoy = 0;
        SalidasHoy = 0;
        foreach (var m in todayMovements)
        {
            if (m.Type == MovementType.Entrada) EntradasHoy += m.Quantity;
            else if (m.Type == MovementType.Salida) SalidasHoy += m.Quantity;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Mobile.ViewModels;

public partial class InventoryViewModel : ViewModelBase
{
    private readonly ProductService? _productService;
    private readonly MovementService? _movementService;

    [ObservableProperty]
    private ObservableCollection<ProductListItemDto> _products = new();

    [ObservableProperty]
    private ProductListItemDto? _selectedProduct;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public InventoryViewModel()
    {
        // Design-time
    }

    public InventoryViewModel(ProductService productService, MovementService movementService)
    {
        _productService = productService;
        _movementService = movementService;
        _ = LoadProductsAsync();
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        if (_productService == null) return;

        IsLoading = true;
        try
        {
            var filter = new ProductListFilter(SearchText, null, false, false);
            var products = await _productService.GetListAsync(filter);
            Products = new ObservableCollection<ProductListItemDto>(products);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load products error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadProductsAsync();
    }
}

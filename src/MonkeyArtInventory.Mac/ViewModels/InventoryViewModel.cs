using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MonkeyArtInventory.Mac.ViewModels;

public partial class InventoryViewModel : ViewModelBase
{
    private readonly ProductService _productService;

    [ObservableProperty]
    private ObservableCollection<ProductListItemDto> _productos = new();

    [ObservableProperty]
    private ProductListItemDto? _selectedProduct;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public InventoryViewModel()
    {
        _productService = App.Services!.GetRequiredService<ProductService>();
        _ = LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        var filter = new ProductListFilter(SearchText, null, false, false);
        var products = await _productService.GetListAsync(filter);
        Productos = new ObservableCollection<ProductListItemDto>(products);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        await LoadProductsAsync();
    }
}

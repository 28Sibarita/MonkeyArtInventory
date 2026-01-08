using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.App.Infrastructure;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Core.Utilities;

namespace MonkeyArtInventory.App.ViewModels;

public class InventoryViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly NavigationStore _navigationStore;
    private readonly Func<ProductDetailViewModel> _productDetailFactory;
    private readonly Func<MovementType, MovementEntryViewModel> _movementFactory;

    private string? _searchText;
    private EnumOption<ProductType>? _selectedType;
    private bool _onlyLowStock;
    private bool _onlyOutOfStock;
    private ProductListItemDto? _selectedProduct;

    public InventoryViewModel(
        ProductService productService,
        NavigationStore navigationStore,
        Func<ProductDetailViewModel> productDetailFactory,
        Func<MovementType, MovementEntryViewModel> movementFactory)
    {
        _productService = productService;
        _navigationStore = navigationStore;
        _productDetailFactory = productDetailFactory;
        _movementFactory = movementFactory;

        TypeOptions = new ObservableCollection<EnumOption<ProductType>>(
            Enum.GetValues<ProductType>()
                .Select(t => new EnumOption<ProductType>(t, EnumHelper.GetDisplayName(t))));

        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        NewProductCommand = new AsyncRelayCommand(CreateNewProductAsync);
        EditProductCommand = new AsyncRelayCommand(EditSelectedProductAsync);
        DeleteProductCommand = new AsyncRelayCommand(DeleteSelectedProductAsync);
        RegisterAdjustmentCommand = new RelayCommand(() => NavigateToMovement(MovementType.Ajuste));
        RegisterLossCommand = new RelayCommand(() => NavigateToMovement(MovementType.Merma));

        _ = LoadAsync();
    }

    public ObservableCollection<ProductListItemDto> Products { get; } = new();

    public ObservableCollection<EnumOption<ProductType>> TypeOptions { get; }

    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public EnumOption<ProductType>? SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    public bool OnlyLowStock
    {
        get => _onlyLowStock;
        set => SetProperty(ref _onlyLowStock, value);
    }

    public bool OnlyOutOfStock
    {
        get => _onlyOutOfStock;
        set => SetProperty(ref _onlyOutOfStock, value);
    }

    public ProductListItemDto? SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand NewProductCommand { get; }
    public IAsyncRelayCommand EditProductCommand { get; }
    public IAsyncRelayCommand DeleteProductCommand { get; }
    public IRelayCommand RegisterAdjustmentCommand { get; }
    public IRelayCommand RegisterLossCommand { get; }

    public async Task LoadAsync()
    {
        var filter = new ProductListFilter(
            SearchText,
            SelectedType?.Value,
            OnlyLowStock,
            OnlyOutOfStock);

        var list = await _productService.GetListAsync(filter);
        Products.Clear();
        foreach (var item in list)
        {
            Products.Add(item);
        }
    }

    private async Task CreateNewProductAsync()
    {
        var vm = _productDetailFactory();
        await vm.LoadAsync(null);
        _navigationStore.CurrentViewModel = vm;
    }

    private async Task EditSelectedProductAsync()
    {
        if (SelectedProduct is null)
        {
            MessageBox.Show("Seleccione un producto primero.", "Inventario", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var vm = _productDetailFactory();
        await vm.LoadAsync(SelectedProduct.Id);
        _navigationStore.CurrentViewModel = vm;
    }

    private async Task DeleteSelectedProductAsync()
    {
        if (SelectedProduct is null)
        {
            MessageBox.Show("Seleccione un producto primero.", "Inventario", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            "¿Eliminar el producto seleccionado?",
            "Inventario",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        var result = await _productService.DeleteAsync(SelectedProduct.Id);
        MessageBox.Show(result.Message, "Inventario", MessageBoxButton.OK, result.Success ? MessageBoxImage.Information : MessageBoxImage.Error);
        await LoadAsync();
    }

    // Called by the view when the barcode scanner (HID keyboard) sends input and the user presses Enter.
    public void HandleScannedBarcode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return;

        var product = Products.FirstOrDefault(p => string.Equals(p.Barcode, code, StringComparison.OrdinalIgnoreCase));
        if (product != null)
        {
            SelectedProduct = product;
            // Start editing/viewing the selected product (fire-and-forget)
            _ = EditSelectedProductAsync();
        }
        else
        {
            MessageBox.Show($"Producto con código {code} no encontrado.", "Inventario", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void NavigateToMovement(MovementType type)
    {
        _navigationStore.CurrentViewModel = _movementFactory(type);
    }
}

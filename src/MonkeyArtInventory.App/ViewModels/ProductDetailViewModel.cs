using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MonkeyArtInventory.App.Infrastructure;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Core.Utilities;

namespace MonkeyArtInventory.App.ViewModels;

public class ProductDetailViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly SettingsService _settingsService;
    private readonly NavigationStore _navigationStore;
    private readonly Func<InventoryViewModel> _inventoryFactory;

    private int _id;
    private string _name = string.Empty;
    private string _barcode = string.Empty;
    private EnumOption<ProductType>? _selectedType;
    private int _stockMinimo;
    private int _currentStock;
    private string? _location;
    private decimal? _salePrice;
    private string? _imagePath;
    private bool _isActive = true;
    private bool _isEditMode;

    public ProductDetailViewModel(
        ProductService productService,
        SettingsService settingsService,
        NavigationStore navigationStore,
        Func<InventoryViewModel> inventoryFactory)
    {
        _productService = productService;
        _settingsService = settingsService;
        _navigationStore = navigationStore;
        _inventoryFactory = inventoryFactory;

        TypeOptions = new ObservableCollection<EnumOption<ProductType>>(
            Enum.GetValues<ProductType>()
                .Select(t => new EnumOption<ProductType>(t, EnumHelper.GetDisplayName(t))));

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        BrowseImageCommand = new RelayCommand(BrowseImage);
    }

    public ObservableCollection<EnumOption<ProductType>> TypeOptions { get; }

    public bool EnablePrices => _settingsService.Current.EnablePrices;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Barcode
    {
        get => _barcode;
        set => SetProperty(ref _barcode, value);
    }

    public EnumOption<ProductType>? SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    public int StockMinimo
    {
        get => _stockMinimo;
        set => SetProperty(ref _stockMinimo, value);
    }

    public int CurrentStock
    {
        get => _currentStock;
        set => SetProperty(ref _currentStock, value);
    }

    public string? Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public decimal? SalePrice
    {
        get => _salePrice;
        set => SetProperty(ref _salePrice, value);
    }

    public string? ImagePath
    {
        get => _imagePath;
        set => SetProperty(ref _imagePath, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand BrowseImageCommand { get; }

    public async Task LoadAsync(int? productId)
    {
        if (productId.HasValue)
        {
            var product = await _productService.GetByIdAsync(productId.Value);
            if (product is null)
            {
                MessageBox.Show("Producto no encontrado.", "Producto", MessageBoxButton.OK, MessageBoxImage.Error);
                Cancel();
                return;
            }

            Id = product.Id;
            Name = product.Name;
            Barcode = product.Barcode;
            SelectedType = TypeOptions.FirstOrDefault(o => o.Value == product.Type);
            StockMinimo = product.StockMinimo;
            CurrentStock = await _productService.GetStockAsync(product.Id);
            Location = product.Location;
            SalePrice = product.SalePrice;
            ImagePath = product.ImagePath;
            IsActive = product.IsActive;
            IsEditMode = true;
        }
        else
        {
            Id = 0;
            Name = string.Empty;
            Barcode = string.Empty;
            SelectedType = TypeOptions.FirstOrDefault();
            StockMinimo = 0;
            CurrentStock = 0;
            Location = string.Empty;
            SalePrice = null;
            ImagePath = null;
            IsActive = true;
            IsEditMode = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedType is null)
        {
            MessageBox.Show("Seleccione un tipo de producto.", "Producto", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var product = new Product
        {
            Id = Id,
            Name = Name.Trim(),
            Barcode = Barcode.Trim(),
            Type = SelectedType.Value,
            StockMinimo = StockMinimo,
            Location = string.IsNullOrWhiteSpace(Location) ? null : Location.Trim(),
            SalePrice = EnablePrices ? SalePrice : null,
            ImagePath = string.IsNullOrWhiteSpace(ImagePath) ? null : ImagePath,
            IsActive = IsActive
        };

        var result = IsEditMode
            ? await _productService.UpdateAsync(product)
            : await _productService.CreateAsync(product);

        MessageBox.Show(result.Message, "Producto", MessageBoxButton.OK, result.Success ? MessageBoxImage.Information : MessageBoxImage.Error);

        if (result.Success)
        {
            _navigationStore.CurrentViewModel = _inventoryFactory();
        }
    }

    private void Cancel()
    {
        _navigationStore.CurrentViewModel = _inventoryFactory();
    }

    private void BrowseImage()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Image",
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            ImagePath = dialog.FileName;
        }
    }
}

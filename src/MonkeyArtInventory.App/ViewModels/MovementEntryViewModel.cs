using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Core.Utilities;

namespace MonkeyArtInventory.App.ViewModels;

public class MovementEntryViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly MovementService _movementService;
    private readonly SettingsService _settingsService;

    private string _barcodeText = string.Empty;
    private Product? _selectedProduct;
    private int _quantity = 1;
    private decimal? _unitPrice;
    private string? _note;
    private string? _clientName;
    private decimal? _clientPrice;
    private bool _isConsignment;
    private string? _clientNote;
    private bool _showClientPanel;
    private int _currentStock;
    private bool _autoSaveOnScan;

    public MovementEntryViewModel(
        MovementType movementType,
        ProductService productService,
        MovementService movementService,
        SettingsService settingsService)
    {
        MovementType = movementType;
        _productService = productService;
        _movementService = movementService;
        _settingsService = settingsService;

        ResolveBarcodeCommand = new AsyncRelayCommand(ResolveBarcodeAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ClearCommand = new RelayCommand(Clear);
        ToggleClientPanelCommand = new RelayCommand(() => ShowClientPanel = !ShowClientPanel);
    }

    public bool AutoSaveOnScan
    {
        get => _autoSaveOnScan;
        set => SetProperty(ref _autoSaveOnScan, value);
    }

    public MovementType MovementType { get; }

    public string Title => $"Registrar {EnumHelper.GetDisplayName(MovementType)}";

    public bool EnablePrices => _settingsService.Current.EnablePrices;

    public string BarcodeText
    {
        get => _barcodeText;
        set => SetProperty(ref _barcodeText, value);
    }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public decimal? UnitPrice
    {
        get => _unitPrice;
        set => SetProperty(ref _unitPrice, value);
    }

    public string? Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public string? ClientName
    {
        get => _clientName;
        set => SetProperty(ref _clientName, value);
    }

    public decimal? ClientPrice
    {
        get => _clientPrice;
        set => SetProperty(ref _clientPrice, value);
    }

    public bool IsConsignment
    {
        get => _isConsignment;
        set => SetProperty(ref _isConsignment, value);
    }

    public string? ClientNote
    {
        get => _clientNote;
        set => SetProperty(ref _clientNote, value);
    }

    public bool ShowClientPanel
    {
        get => _showClientPanel;
        set => SetProperty(ref _showClientPanel, value);
    }

    public int CurrentStock
    {
        get => _currentStock;
        set => SetProperty(ref _currentStock, value);
    }

    public IAsyncRelayCommand ResolveBarcodeCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand ClearCommand { get; }
    public IRelayCommand ToggleClientPanelCommand { get; }

    private async Task ResolveBarcodeAsync()
    {
        if (string.IsNullOrWhiteSpace(BarcodeText))
        {
            MessageBox.Show("Ingrese un código de barras.", "Movimiento", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var product = await _productService.GetByBarcodeAsync(BarcodeText.Trim());
        if (product is null)
        {
            MessageBox.Show("Producto no encontrado.", "Movimiento", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SelectedProduct = product;
        Quantity = 1;
        CurrentStock = await _productService.GetStockAsync(product.Id);

        if (AutoSaveOnScan)
        {
            // If autoguardar está activado, registrar movimiento inmediatamente
            await SaveAsync();
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedProduct is null)
        {
            MessageBox.Show("Seleccione un producto primero.", "Movimiento", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dto = new MovementCreateDto(
            SelectedProduct.Id,
            MovementType,
            Quantity,
            EnablePrices ? UnitPrice : null,
            string.IsNullOrWhiteSpace(Note) ? null : Note.Trim(),
            string.IsNullOrWhiteSpace(ClientName) ? null : ClientName.Trim(),
            ClientPrice,
            IsConsignment,
            string.IsNullOrWhiteSpace(ClientNote) ? null : ClientNote.Trim(),
            DateTime.Now);

        var result = await _movementService.RegisterMovementAsync(dto);
        MessageBox.Show(result.Message, "Movimiento", MessageBoxButton.OK, result.Success ? MessageBoxImage.Information : MessageBoxImage.Error);

        if (result.Success)
        {
            Clear();
        }
    }

    private void Clear()
    {
        BarcodeText = string.Empty;
        SelectedProduct = null;
        Quantity = 1;
        UnitPrice = null;
        Note = null;
        CurrentStock = 0;
    }
}

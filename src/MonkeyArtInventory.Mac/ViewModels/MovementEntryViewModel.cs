using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Services;
using MonkeyArtInventory.Core.Utilities;

namespace MonkeyArtInventory.Mac.ViewModels;

public partial class MovementEntryViewModel : ViewModelBase
{
    private readonly ProductService _productService;
    private readonly MovementService _movementService;
    private readonly SettingsService _settingsService;
    private readonly ClientService _clientService;

    [ObservableProperty]
    private string _barcodeText = string.Empty;

    [ObservableProperty]
    private Product? _selectedProduct;

    [ObservableProperty]
    private int _quantity = 1;

    [ObservableProperty]
    private decimal? _unitPrice;

    [ObservableProperty]
    private string? _note;

    [ObservableProperty]
    private string? _clientName;

    [ObservableProperty]
    private decimal? _clientPrice;

    [ObservableProperty]
    private bool _isConsignment;

    [ObservableProperty]
    private string? _clientNote;

    [ObservableProperty]
    private bool _showClientPanel;

    [ObservableProperty]
    private int _currentStock;

    [ObservableProperty]
    private bool _autoSaveOnScan;

    [ObservableProperty]
    private ClientDto? _selectedClient;

    [ObservableProperty]
    private DestinationType _selectedDestination = DestinationType.Client;

    [ObservableProperty]
    private ObservableCollection<ClientDto> _clients = new();

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isStatusError;

    public MovementType MovementType { get; }

    public string Title => $"Registrar {EnumHelper.GetDisplayName(MovementType)}";

    public bool IsExitMovement => MovementType == MovementType.Salida;

    public bool EnablePrices => _settingsService.Current.EnablePrices;

    public IEnumerable<EnumOption<DestinationType>> DestinationTypes => EnumHelper.GetEnumOptions<DestinationType>();

    public MovementEntryViewModel(
        MovementType movementType,
        ProductService productService,
        MovementService movementService,
        SettingsService settingsService,
        ClientService clientService)
    {
        MovementType = movementType;
        _productService = productService;
        _movementService = movementService;
        _settingsService = settingsService;
        _clientService = clientService;

        if (movementType == MovementType.Salida)
        {
            ShowClientPanel = true;
            _ = LoadClientsAsync();
        }
    }

    private async Task LoadClientsAsync()
    {
        var clients = await _clientService.GetActiveClientsForDropdownAsync();
        Clients = new ObservableCollection<ClientDto>(clients);
    }

    [RelayCommand]
    private async Task ResolveBarcodeAsync()
    {
        if (string.IsNullOrWhiteSpace(BarcodeText)) return;

        var product = await _productService.GetByBarcodeAsync(BarcodeText);
        if (product != null)
        {
            SelectedProduct = product;
            CurrentStock = await _movementService.GetStockAsync(product.Id);
            UnitPrice = product.SalePrice;

            if (AutoSaveOnScan && SelectedProduct != null)
            {
                await SaveAsync();
            }
        }
        else
        {
            ShowStatus("Producto no encontrado", true);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedProduct == null)
        {
            ShowStatus("Seleccione un producto primero", true);
            return;
        }

        if (Quantity <= 0)
        {
            ShowStatus("La cantidad debe ser mayor a 0", true);
            return;
        }

        var dto = new MovementCreateDto(
            SelectedProduct.Id,
            MovementType,
            Quantity,
            UnitPrice,
            Note,
            ClientName,
            ClientPrice,
            IsConsignment,
            ClientNote,
            null,  // OccurredAt - let service default to now
            SelectedClient?.Id,
            IsExitMovement ? SelectedDestination : null
        );

        var result = await _movementService.RegisterMovementAsync(dto);
        if (result.Success)
        {
            ShowStatus("Movimiento registrado exitosamente", false);
            Clear();
        }
        else
        {
            ShowStatus(result.Message, true);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        BarcodeText = string.Empty;
        SelectedProduct = null;
        Quantity = 1;
        UnitPrice = null;
        Note = null;
        ClientName = null;
        ClientPrice = null;
        IsConsignment = false;
        ClientNote = null;
        CurrentStock = 0;
        SelectedClient = null;
        SelectedDestination = DestinationType.Client;
        StatusMessage = null;
    }

    [RelayCommand]
    private void ToggleClientPanel()
    {
        ShowClientPanel = !ShowClientPanel;
    }

    private void ShowStatus(string message, bool isError)
    {
        StatusMessage = message;
        IsStatusError = isError;
    }

    partial void OnSelectedClientChanged(ClientDto? value)
    {
        if (value != null)
        {
            ClientName = value.Name;
        }
    }
}

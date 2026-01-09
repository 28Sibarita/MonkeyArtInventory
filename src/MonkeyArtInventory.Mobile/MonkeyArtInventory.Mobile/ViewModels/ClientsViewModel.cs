using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Mobile.ViewModels;

public partial class ClientsViewModel : ViewModelBase
{
    private readonly ClientService _clientService;

    [ObservableProperty]
    private ObservableCollection<ClientDto> _clients = new();

    [ObservableProperty]
    private ClientDto? _selectedClient;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showInactive;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private int _editingClientId;

    // Form fields
    [ObservableProperty]
    private string _clientName = string.Empty;

    [ObservableProperty]
    private string _clientPhone = string.Empty;

    [ObservableProperty]
    private string _clientEmail = string.Empty;

    [ObservableProperty]
    private string _clientAddress = string.Empty;

    [ObservableProperty]
    private string _clientNotes = string.Empty;

    [ObservableProperty]
    private bool _isConsignmentClient;

    [ObservableProperty]
    private decimal _defaultDiscount;

    [ObservableProperty]
    private bool _clientIsActive = true;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isStatusError;

    public ClientsViewModel(ClientService clientService)
    {
        _clientService = clientService;
        _ = LoadClientsAsync();
    }

    [RelayCommand]
    private async Task LoadClientsAsync()
    {
        var clients = await _clientService.GetListAsync(SearchText, ShowInactive);
        Clients = new ObservableCollection<ClientDto>(clients);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadClientsAsync();
    }

    [RelayCommand]
    private void NewClient()
    {
        ClearForm();
        IsEditing = true;
        ClientIsActive = true;
    }

    [RelayCommand]
    private void EditClient()
    {
        if (SelectedClient == null) return;

        EditingClientId = SelectedClient.Id;
        ClientName = SelectedClient.Name;
        ClientPhone = SelectedClient.ContactPhone ?? string.Empty;
        ClientEmail = SelectedClient.ContactEmail ?? string.Empty;
        ClientAddress = SelectedClient.Address ?? string.Empty;
        ClientNotes = SelectedClient.Notes ?? string.Empty;
        IsConsignmentClient = SelectedClient.IsConsignmentClient;
        DefaultDiscount = SelectedClient.DefaultDiscountPercent ?? 0;
        ClientIsActive = SelectedClient.IsActive;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveClientAsync()
    {
        if (string.IsNullOrWhiteSpace(ClientName))
        {
            ShowStatus("El nombre del cliente es requerido", true);
            return;
        }

        ServiceResult result;

        if (EditingClientId == 0)
        {
            var createDto = new ClientCreateDto(
                ClientName,
                string.IsNullOrWhiteSpace(ClientPhone) ? null : ClientPhone,
                string.IsNullOrWhiteSpace(ClientEmail) ? null : ClientEmail,
                string.IsNullOrWhiteSpace(ClientAddress) ? null : ClientAddress,
                string.IsNullOrWhiteSpace(ClientNotes) ? null : ClientNotes,
                IsConsignmentClient,
                DefaultDiscount
            );
            result = await _clientService.CreateAsync(createDto);
        }
        else
        {
            var updateDto = new ClientUpdateDto(
                EditingClientId,
                ClientName,
                string.IsNullOrWhiteSpace(ClientPhone) ? null : ClientPhone,
                string.IsNullOrWhiteSpace(ClientEmail) ? null : ClientEmail,
                string.IsNullOrWhiteSpace(ClientAddress) ? null : ClientAddress,
                string.IsNullOrWhiteSpace(ClientNotes) ? null : ClientNotes,
                ClientIsActive,
                IsConsignmentClient,
                DefaultDiscount
            );
            result = await _clientService.UpdateAsync(updateDto);
        }

        if (result.Success)
        {
            ShowStatus(EditingClientId == 0 ? "Cliente creado" : "Cliente actualizado", false);
            ClearForm();
            IsEditing = false;
            await LoadClientsAsync();
        }
        else
        {
            ShowStatus(result.Message ?? "Error al guardar", true);
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        ClearForm();
        IsEditing = false;
    }

    [RelayCommand]
    private async Task DeleteClientAsync()
    {
        if (SelectedClient == null) return;

        var result = await _clientService.DeleteAsync(SelectedClient.Id);
        if (result.Success)
        {
            ShowStatus("Cliente eliminado", false);
            await LoadClientsAsync();
        }
        else
        {
            ShowStatus(result.Message ?? "Error al eliminar", true);
        }
    }

    private void ClearForm()
    {
        EditingClientId = 0;
        ClientName = string.Empty;
        ClientPhone = string.Empty;
        ClientEmail = string.Empty;
        ClientAddress = string.Empty;
        ClientNotes = string.Empty;
        IsConsignmentClient = false;
        DefaultDiscount = 0;
        ClientIsActive = true;
        StatusMessage = null;
    }

    private void ShowStatus(string message, bool isError)
    {
        StatusMessage = message;
        IsStatusError = isError;
    }

    partial void OnShowInactiveChanged(bool value)
    {
        _ = LoadClientsAsync();
    }
}

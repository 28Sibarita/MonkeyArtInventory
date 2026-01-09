using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Services;
using System.Collections.ObjectModel;

namespace MonkeyArtInventory.App.ViewModels;

public partial class ClientsViewModel : ObservableObject
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

    // Form fields for adding/editing
    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private int _editingClientId;

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
    private decimal? _defaultDiscount;

    [ObservableProperty]
    private bool _clientIsActive = true;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

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
        EditingClientId = 0;
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
        DefaultDiscount = SelectedClient.DefaultDiscountPercent;
        ClientIsActive = SelectedClient.IsActive;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveClientAsync()
    {
        if (string.IsNullOrWhiteSpace(ClientName))
        {
            ShowStatus("El nombre del cliente es obligatorio", true);
            return;
        }

        ServiceResult result;

        if (EditingClientId == 0)
        {
            // Create new client
            var dto = new ClientCreateDto(
                ClientName.Trim(),
                string.IsNullOrWhiteSpace(ClientPhone) ? null : ClientPhone.Trim(),
                string.IsNullOrWhiteSpace(ClientEmail) ? null : ClientEmail.Trim(),
                string.IsNullOrWhiteSpace(ClientAddress) ? null : ClientAddress.Trim(),
                string.IsNullOrWhiteSpace(ClientNotes) ? null : ClientNotes.Trim(),
                IsConsignmentClient,
                DefaultDiscount);

            result = await _clientService.CreateAsync(dto);
        }
        else
        {
            // Update existing client
            var dto = new ClientUpdateDto(
                EditingClientId,
                ClientName.Trim(),
                string.IsNullOrWhiteSpace(ClientPhone) ? null : ClientPhone.Trim(),
                string.IsNullOrWhiteSpace(ClientEmail) ? null : ClientEmail.Trim(),
                string.IsNullOrWhiteSpace(ClientAddress) ? null : ClientAddress.Trim(),
                string.IsNullOrWhiteSpace(ClientNotes) ? null : ClientNotes.Trim(),
                ClientIsActive,
                IsConsignmentClient,
                DefaultDiscount);

            result = await _clientService.UpdateAsync(dto);
        }

        if (result.Success)
        {
            ShowStatus(EditingClientId == 0 ? "Cliente creado exitosamente" : "Cliente actualizado exitosamente", false);
            ClearForm();
            IsEditing = false;
            await LoadClientsAsync();
        }
        else
        {
            ShowStatus(result.Message ?? "Error al guardar el cliente", true);
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
            ShowStatus("Cliente eliminado/desactivado exitosamente", false);
            await LoadClientsAsync();
        }
        else
        {
            ShowStatus(result.Message ?? "Error al eliminar el cliente", true);
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
        DefaultDiscount = null;
        ClientIsActive = true;
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

    partial void OnSelectedClientChanged(ClientDto? value)
    {
        if (value != null && !IsEditing)
        {
            // Show client details
        }
    }
}

using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Services;

public class ClientService
{
    private readonly IInventoryRepository _repository;

    public ClientService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ClientDto>> GetListAsync(string? searchText = null, bool includeInactive = false)
    {
        var clients = await _repository.ListClientsAsync();
        var movements = await _repository.ListMovementsAsync();

        // Calculate stats per client
        var clientStats = movements
            .Where(m => m.ClientId.HasValue && m.Type == MovementType.Salida)
            .GroupBy(m => m.ClientId!.Value)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalPurchases = g.Count(),
                    TotalSpent = g.Sum(m => (m.ClientPrice ?? m.UnitPrice ?? 0) * m.Quantity)
                });

        IEnumerable<Client> query = clients;

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Name.ToLowerInvariant().Contains(term) ||
                (c.ContactEmail?.ToLowerInvariant().Contains(term) ?? false) ||
                (c.ContactPhone?.Contains(term) ?? false));
        }

        return query
            .OrderBy(c => c.Name)
            .Select(c =>
            {
                var stats = clientStats.TryGetValue(c.Id, out var s) ? s : null;
                return new ClientDto(
                    c.Id,
                    c.Name,
                    c.ContactPhone,
                    c.ContactEmail,
                    c.Address,
                    c.Notes,
                    c.IsActive,
                    c.IsConsignmentClient,
                    c.DefaultDiscountPercent,
                    c.CreatedAt,
                    stats?.TotalPurchases ?? 0,
                    stats?.TotalSpent ?? 0);
            })
            .ToList();
    }

    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        var client = await _repository.FindClientByIdAsync(id);
        if (client == null) return null;

        var movements = await _repository.GetClientMovementsAsync(id);
        var sales = movements.Where(m => m.Type == MovementType.Salida).ToList();

        return new ClientDto(
            client.Id,
            client.Name,
            client.ContactPhone,
            client.ContactEmail,
            client.Address,
            client.Notes,
            client.IsActive,
            client.IsConsignmentClient,
            client.DefaultDiscountPercent,
            client.CreatedAt,
            sales.Count,
            sales.Sum(m => (m.ClientPrice ?? m.UnitPrice ?? 0) * m.Quantity));
    }

    public async Task<ServiceResult> CreateAsync(ClientCreateDto dto)
    {
        // Check if client name already exists
        var existing = await _repository.FindClientByNameAsync(dto.Name);
        if (existing != null)
        {
            return ServiceResult.Fail($"Ya existe un cliente con el nombre '{dto.Name}'");
        }

        var client = new Client
        {
            Name = dto.Name.Trim(),
            ContactPhone = dto.ContactPhone?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            Address = dto.Address?.Trim(),
            Notes = dto.Notes?.Trim(),
            IsConsignmentClient = dto.IsConsignmentClient,
            DefaultDiscountPercent = dto.DefaultDiscountPercent,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        await _repository.AddClientAsync(client);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(ClientUpdateDto dto)
    {
        var client = await _repository.FindClientByIdAsync(dto.Id);
        if (client == null)
        {
            return ServiceResult.Fail("Cliente no encontrado");
        }

        // Check if new name conflicts with another client
        var existing = await _repository.FindClientByNameAsync(dto.Name);
        if (existing != null && existing.Id != dto.Id)
        {
            return ServiceResult.Fail($"Ya existe otro cliente con el nombre '{dto.Name}'");
        }

        client.Name = dto.Name.Trim();
        client.ContactPhone = dto.ContactPhone?.Trim();
        client.ContactEmail = dto.ContactEmail?.Trim();
        client.Address = dto.Address?.Trim();
        client.Notes = dto.Notes?.Trim();
        client.IsActive = dto.IsActive;
        client.IsConsignmentClient = dto.IsConsignmentClient;
        client.DefaultDiscountPercent = dto.DefaultDiscountPercent;

        await _repository.UpdateClientAsync(client);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var client = await _repository.FindClientByIdAsync(id);
        if (client == null)
        {
            return ServiceResult.Fail("Cliente no encontrado");
        }

        // Check if client has movements
        var movements = await _repository.GetClientMovementsAsync(id);
        if (movements.Any())
        {
            // Soft delete - just deactivate
            client.IsActive = false;
            await _repository.UpdateClientAsync(client);
            await _repository.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        await _repository.DeleteClientAsync(client);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<IReadOnlyList<ClientDto>> GetActiveClientsForDropdownAsync()
    {
        var clients = await _repository.ListClientsAsync();
        return clients
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new ClientDto(
                c.Id,
                c.Name,
                c.ContactPhone,
                c.ContactEmail,
                c.Address,
                c.Notes,
                c.IsActive,
                c.IsConsignmentClient,
                c.DefaultDiscountPercent,
                c.CreatedAt,
                0, 0))
            .ToList();
    }
}

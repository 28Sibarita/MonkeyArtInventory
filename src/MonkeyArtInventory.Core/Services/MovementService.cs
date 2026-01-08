using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Services;

public class MovementService
{
    private readonly IInventoryRepository _repository;

    public MovementService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult> RegisterMovementAsync(MovementCreateDto dto)
    {
        if (dto.Quantity <= 0)
        {
            return ServiceResult.Fail("Quantity must be greater than zero.");
        }

        var product = await _repository.FindProductByIdAsync(dto.ProductId);
        if (product is null)
        {
            return ServiceResult.Fail("Product not found.");
        }

        var currentStock = await GetStockAsync(dto.ProductId);
        var delta = IsIncrease(dto.Type) ? dto.Quantity : -dto.Quantity;
        var nextStock = currentStock + delta;

        if (nextStock < 0 && (dto.Type == MovementType.Salida || dto.Type == MovementType.Merma))
        {
            return ServiceResult.Fail("Insufficient stock. Use Ajuste to correct stock if needed.");
        }

        var movement = new Movement
        {
            ProductId = dto.ProductId,
            Type = dto.Type,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            Note = dto.Note,
            ClientName = dto.ClientName,
            ClientPrice = dto.ClientPrice,
            IsConsignment = dto.IsConsignment,
            ClientNote = dto.ClientNote,
            OccurredAt = dto.OccurredAt ?? DateTime.Now
        };

        await _repository.AddMovementAsync(movement);
        await _repository.SaveChangesAsync();
        return ServiceResult.Ok("Movement registered.");
    }

    public async Task<int> GetStockAsync(int productId)
    {
        var movements = await _repository.ListMovementsAsync();
        return movements
            .Where(m => m.ProductId == productId)
            .Sum(m => IsIncrease(m.Type) ? m.Quantity : -m.Quantity);
    }

    public async Task<int> GetTodayMovementCountAsync()
    {
        var today = DateTime.Today;
        var movements = await _repository.ListMovementsAsync(today, today.AddDays(1));
        return movements.Count;
    }

    public async Task<IReadOnlyList<Movement>> GetTodayMovementsAsync()
    {
        var today = DateTime.Today;
        var movements = await _repository.ListMovementsAsync(today, today.AddDays(1));
        return movements;
    }

    private static bool IsIncrease(MovementType type)
    {
        return type == MovementType.Entrada || type == MovementType.Ajuste;
    }
}

using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.DTOs;

public record MovementCreateDto(
    int ProductId,
    MovementType Type,
    int Quantity,
    decimal? UnitPrice,
    string? Note,
    string? ClientName,
    decimal? ClientPrice,
    bool IsConsignment,
    string? ClientNote,
    DateTime? OccurredAt);

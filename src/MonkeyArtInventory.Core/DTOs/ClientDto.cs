namespace MonkeyArtInventory.Core.DTOs;

public record ClientDto(
    int Id,
    string Name,
    string? ContactPhone,
    string? ContactEmail,
    string? Address,
    string? Notes,
    bool IsActive,
    bool IsConsignmentClient,
    decimal? DefaultDiscountPercent,
    DateTime CreatedAt,
    int TotalPurchases,
    decimal TotalSpent);

public record ClientCreateDto(
    string Name,
    string? ContactPhone,
    string? ContactEmail,
    string? Address,
    string? Notes,
    bool IsConsignmentClient,
    decimal? DefaultDiscountPercent);

public record ClientUpdateDto(
    int Id,
    string Name,
    string? ContactPhone,
    string? ContactEmail,
    string? Address,
    string? Notes,
    bool IsActive,
    bool IsConsignmentClient,
    decimal? DefaultDiscountPercent);

using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.DTOs;

public record ProductListFilter(
    string? Query,
    ProductType? Type,
    bool OnlyLowStock,
    bool OnlyOutOfStock);

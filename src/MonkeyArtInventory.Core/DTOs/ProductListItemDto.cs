namespace MonkeyArtInventory.Core.DTOs;

public record ProductListItemDto(
    int Id,
    string Name,
    string Barcode,
    string TypeLabel,
    int Stock,
    int StockMinimo,
    string? Location,
    decimal? SalePrice,
    string? ImagePath,
    bool IsActive);

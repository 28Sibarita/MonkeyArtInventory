namespace MonkeyArtInventory.Core.DTOs;

public record WeeklyReportTotalsDto(
    int TotalEntries,
    int TotalExits,
    decimal TotalEstimatedSales,
    decimal TotalSalesValue,
    decimal TotalReturnsValue);

public record WeeklyReportTopItemDto(
    string ProductName,
    int Quantity);

public record WeeklyReportStockItemDto(
    string ProductName,
    string Barcode,
    int Stock);

public record WeeklyReportClientItemDto(
    string ClientName,
    string ProductName,
    string MovementType,
    int Quantity,
    decimal? UnitPrice,
    decimal? TotalValue,
    string? Note);

public class WeeklyReportDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
        public WeeklyReportTotalsDto Totals { get; set; } = new(0, 0, 0m, 0m, 0m);
    public List<WeeklyReportTopItemDto> TopProducts { get; set; } = new();
    public List<WeeklyReportTopItemDto> TopReturns { get; set; } = new();
    public List<WeeklyReportStockItemDto> StockByProduct { get; set; } = new();
    public List<WeeklyReportClientItemDto> ClientMovements { get; set; } = new();
}

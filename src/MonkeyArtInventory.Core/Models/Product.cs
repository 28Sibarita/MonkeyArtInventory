using System.ComponentModel.DataAnnotations;

namespace MonkeyArtInventory.Core.Models;

public class Product
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public ProductType Type { get; set; }

    [MaxLength(100)]
    public string Barcode { get; set; } = string.Empty;

    public int StockMinimo { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public decimal? SalePrice { get; set; }

    [MaxLength(260)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Movement> Movements { get; set; } = new List<Movement>();
}

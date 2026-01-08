using System.ComponentModel.DataAnnotations;

namespace MonkeyArtInventory.Core.Models;

public class Movement
{
    public int Id { get; set; }

    public MovementType Type { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.Now;

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public decimal? UnitPrice { get; set; }

    // Client-related fields (will be persisted after applying EF migration)
    [MaxLength(200)]
    public string? ClientName { get; set; }

    public decimal? ClientPrice { get; set; }

    public bool IsConsignment { get; set; }

    [MaxLength(500)]
    public string? ClientNote { get; set; }
}

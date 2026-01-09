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

    // Client relationship
    public int? ClientId { get; set; }
    public Client? Client { get; set; }

    // Client-related fields for quick access (denormalized for reports)
    [MaxLength(200)]
    public string? ClientName { get; set; }

    public decimal? ClientPrice { get; set; }

    public bool IsConsignment { get; set; }

    [MaxLength(500)]
    public string? ClientNote { get; set; }

    /// <summary>
    /// Destination type for exits: Client, Internal, Other
    /// </summary>
    public DestinationType? Destination { get; set; }
}

/// <summary>
/// Destination type for product exits
/// </summary>
public enum DestinationType
{
    [Display(Name = "Cliente")]
    Client = 0,

    [Display(Name = "Uso Interno")]
    Internal = 1,

    [Display(Name = "Regalo/Muestra")]
    Gift = 2,

    [Display(Name = "Otro")]
    Other = 3
}

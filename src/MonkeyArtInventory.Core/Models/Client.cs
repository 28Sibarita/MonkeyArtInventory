using System.ComponentModel.DataAnnotations;

namespace MonkeyArtInventory.Core.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ContactPhone { get; set; }

    [MaxLength(200)]
    public string? ContactEmail { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Indicates if this is a consignment client (products on loan)
    /// </summary>
    public bool IsConsignmentClient { get; set; }

    /// <summary>
    /// Default discount percentage for this client
    /// </summary>
    public decimal? DefaultDiscountPercent { get; set; }

    // Navigation property
    public ICollection<Movement> Movements { get; set; } = new List<Movement>();
}

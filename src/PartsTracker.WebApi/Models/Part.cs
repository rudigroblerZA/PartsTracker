using System.ComponentModel.DataAnnotations;

namespace PartsTracker.WebApi.Models;

/// <summary>
/// Represents an inventory part in the system.
/// </summary>
public class Part
{
    /// <summary>
    /// Gets or sets the unique identifier for the part.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the part.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the part currently on hand.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "QuantityOnHand must be zero or more.")]
    public int QuantityOnHand { get; set; }

    /// <summary>
    /// Gets or sets the location code where the part is stored.
    /// </summary>
    [MaxLength(50)]
    public string? LocationCode { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last stock take.
    /// </summary>
    public DateTime LastStockTake { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the name of the part.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

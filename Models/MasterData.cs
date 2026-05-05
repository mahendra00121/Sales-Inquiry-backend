using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class MasterData
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // e.g. "ProductType", "MaterialPreference"

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? UpdatedAt { get; set; }
}

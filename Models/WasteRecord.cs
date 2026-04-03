using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class WasteRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ShopFloorId { get; set; }

    [ForeignKey("ShopFloorId")]
    public ShopFloor? ShopFloorEntry { get; set; }

    [Required]
    public string WasteType { get; set; } = "Trim Scrap"; // Trim Scrap, Rejection, Purge

    [Required]
    public decimal Weight { get; set; }

    [Required]
    public string Material { get; set; } = "Mixed"; // PET, PVC, PP

    [Required]
    public string ActionTaken { get; set; } = "Pending"; // Pending, Recycled, Sold, Disposed

    public DateTime RecordedAt { get; set; } = DateTime.Now;

    public string? Remarks { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class ShopFloor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductionPlanId { get; set; }

    [ForeignKey("ProductionPlanId")]
    public ProductionPlan? ProductionPlan { get; set; }

    [Required]
    public string BatchNumber { get; set; } = string.Empty;

    [Required]
    public int ActualQuantityProduced { get; set; }

    public decimal? MaterialConsumedQuantity { get; set; }

    public int WasteQuantity { get; set; } = 0;

    [Required]
    public string OperatorName { get; set; } = string.Empty;

    [Required]
    public string Shift { get; set; } = "Day"; // Day, Night

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? ProductionNotes { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.Now;
}

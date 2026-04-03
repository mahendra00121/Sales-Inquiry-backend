using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class FinalQC
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductionRecordId { get; set; }

    [ForeignKey("ProductionRecordId")]
    public ShopFloor? ShopFloorRecord { get; set; }

    public string? BatchNumber { get; set; }

    [Required]
    public int TestedQuantity { get; set; }

    [Required]
    public int PassedQuantity { get; set; }

    public int RejectedQuantity { get; set; } = 0;

    [Required]
    public string QCIncharge { get; set; } = string.Empty;

    public DateTime TestDate { get; set; } = DateTime.Now;

    [Required]
    public string Outcome { get; set; } = "Pass"; // Pass, Fail, Rework

    public string? QCNotes { get; set; }
}

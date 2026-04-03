using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class Procurement
{
    [Key]
    public int Id { get; set; }

    public int? ProductionPlanId { get; set; }

    [ForeignKey("ProductionPlanId")]
    public ProductionPlan? ProductionPlan { get; set; }

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    public string? PurchaseOrderNumber { get; set; }

    public string? VendorName { get; set; }

    [Required]
    public decimal Quantity { get; set; }

    [MaxLength(20)]
    public string Unit { get; set; } = "kg"; // kg, meter, pieces etc.

    public DateTime? ExpectedDate { get; set; }

    [Required]
    public string QCStatus { get; set; } = "Pending"; // Pending, Approved, Rejected, On-Hold

    public string? QCNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

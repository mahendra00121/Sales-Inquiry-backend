using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class ProductionPlan
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public SalesOrder? Order { get; set; }

    [Required]
    public string MachineName { get; set; } = string.Empty;

    [Required]
    public DateTime PlannedStartDate { get; set; }

    public DateTime? PlannedEndDate { get; set; }

    [Required]
    public int TargetQuantity { get; set; }

    public int CompletedQuantity { get; set; } = 0;

    public string Priority { get; set; } = "Medium"; // High, Medium, Low

    [Required]
    public string Status { get; set; } = "Scheduled"; // Scheduled, In-Progress, Completed, Paused

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

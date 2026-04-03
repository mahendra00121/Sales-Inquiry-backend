using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class Dispatch
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PackingId { get; set; }

    [ForeignKey("PackingId")]
    public Packing? PackingRecord { get; set; }

    [Required]
    public DateTime DispatchDate { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(20)]
    public string VehicleNumber { get; set; } = string.Empty;

    public string? DriverName { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public string DestinationAddress { get; set; } = string.Empty;

    public string? CarrierName { get; set; }

    public string? TrackingId { get; set; }

    public string DispatchStatus { get; set; } = "In-Transit"; // In-Transit, Delivered, Delayed

    public decimal? TotalDispatchWeight { get; set; }

    public string? Remarks { get; set; }
}

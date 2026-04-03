using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class SalesInquiry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    public string? ContactPerson { get; set; }
    
    public string? ContactEmail { get; set; }
    
    public string? ContactNumber { get; set; }

    [Required]
    public DateTime InquiryDate { get; set; } = DateTime.Now;

    [Required]
    public string Description { get; set; } = string.Empty;

    public int? QuantityRequested { get; set; }

    [Required]
    public string Status { get; set; } = "New"; // New, Feasibility, Costing, Order, Rejected

    public string? LeadSource { get; set; }

    public string? InquiryReference { get; set; }

    public string? AssignedTo { get; set; }

    public DateTime? CustomerExpectedDate { get; set; }

    public string? Remarks { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

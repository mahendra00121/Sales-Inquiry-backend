using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class FeasibilityReview
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InquiryId { get; set; }

    [ForeignKey("InquiryId")]
    public SalesInquiry? Inquiry { get; set; }

    [Required]
    public bool IsFeasible { get; set; }

    public string? TechnicalNotes { get; set; }

    public int? EstimatedProcessDays { get; set; }

    [Required]
    public string ReviewedBy { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; } = DateTime.Now;
    
    public string? Status { get; set; } // Approved, Rejected, Conditional
}

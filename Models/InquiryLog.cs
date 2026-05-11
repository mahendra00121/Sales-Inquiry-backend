using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class InquiryLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InquiryId { get; set; }

    [ForeignKey("InquiryId")]
    public SalesInquiry? Inquiry { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty; // Created, Updated, Verified, etc.

    public string? ModifiedBy { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public string? Details { get; set; } // JSON or text describing changes
}

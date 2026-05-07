using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Type { get; set; } // e.g., "Inquiry", "Order", "System"

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public string? RelatedId { get; set; } // e.g., InquiryId
}

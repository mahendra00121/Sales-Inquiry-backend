using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class EmailTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g. OTP_Verification, Inquiry_Confirmation

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty; // HTML content with placeholders like {{CustomerName}}, {{OTP}}
}

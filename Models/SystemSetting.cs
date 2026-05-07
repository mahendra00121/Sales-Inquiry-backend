using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class SystemSetting
{
    [Key]
    public string SettingKey { get; set; } = string.Empty; // e.g., "OTP_EXPIRY_MINUTES"

    [Required]
    public string SettingValue { get; set; } = string.Empty; // e.g., "10"

    public string? Description { get; set; }

    public string? Group { get; set; } // e.g., "Security", "Email", "Company"

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

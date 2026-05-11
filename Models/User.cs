using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }
    
    public string? Email { get; set; }
    
    public string? Role { get; set; }

    public string? ResetOtp { get; set; }

    public DateTime? ResetOtpExpiry { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastActiveAt { get; set; }
}

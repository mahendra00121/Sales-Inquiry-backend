using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }
    
    public string? Role { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

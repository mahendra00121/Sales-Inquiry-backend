using System.ComponentModel.DataAnnotations;

namespace PolyTrack.API.Models;

public class RolePermission
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string RoleName { get; set; } = string.Empty; // Admin, Sales, Production, User

    [Required]
    public string ModuleName { get; set; } = string.Empty; // Overview, Pre-Sales, Production, Logistics, Sustainability, Configuration

    public bool IsVisible { get; set; } = true;
}

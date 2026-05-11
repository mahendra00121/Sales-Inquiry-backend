using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolePermissionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RolePermissionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolePermission>>> GetPermissions()
    {
        return await _context.RolePermissions.ToListAsync();
    }

    [HttpPut("update-bulk")]
    public async Task<IActionResult> UpdateBulkPermissions([FromBody] List<RolePermission> permissions)
    {
        foreach (var p in permissions)
        {
            var existing = await _context.RolePermissions
                .FirstOrDefaultAsync(x => x.RoleName == p.RoleName && x.ModuleName == p.ModuleName);

            if (existing != null)
            {
                existing.IsVisible = p.IsVisible;
            }
            else
            {
                _context.RolePermissions.Add(p);
            }
        }

        await _context.SaveChangesAsync();

        // Add a notification for the system change
        _context.Notifications.Add(new Notification
        {
            Message = "System role permissions have been updated.",
            Type = "System",
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.RolePermissions.AnyAsync()) return BadRequest("Already seeded");

        var roles = new[] { "Admin", "Sales", "Production", "User" };
        var modules = new[] { "Overview", "Pre-Sales", "Production", "Logistics", "Sustainability", "Configuration" };

        var list = new List<RolePermission>();
        foreach (var r in roles)
        {
            foreach (var m in modules)
            {
                bool isVisible = r == "Admin"; // Admin sees all
                if (r == "Sales" && (m == "Overview" || m == "Pre-Sales" || m == "Sustainability")) isVisible = true;
                if (r == "Production" && (m == "Overview" || m == "Production" || m == "Logistics" || m == "Sustainability")) isVisible = true;
                if (r == "User" && (m == "Overview" || m == "Pre-Sales")) isVisible = true;

                list.Add(new RolePermission { RoleName = r, ModuleName = m, IsVisible = isVisible });
            }
        }

        _context.RolePermissions.AddRange(list);
        await _context.SaveChangesAsync();
        return Ok("Seeded");
    }
}

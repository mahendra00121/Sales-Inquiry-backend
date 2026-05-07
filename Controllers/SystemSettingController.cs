using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemSettingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SystemSettingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemSetting>>> GetSettings()
    {
        return await _context.SystemSettings.ToListAsync();
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<SystemSetting>> GetSetting(string key)
    {
        var setting = await _context.SystemSettings.FindAsync(key);
        if (setting == null) return NotFound();
        return setting;
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] SystemSetting setting)
    {
        if (key != setting.SettingKey) return BadRequest();

        setting.UpdatedAt = DateTime.Now;
        _context.Entry(setting).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SettingExists(key)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.SystemSettings.AnyAsync()) return BadRequest("Settings already seeded");

        var settings = new List<SystemSetting>
        {
            new SystemSetting { SettingKey = "OTP_EXPIRY_MINUTES", SettingValue = "10", Description = "Time in minutes for OTP validity", Group = "Security" },
            new SystemSetting { SettingKey = "OTP_LENGTH", SettingValue = "6", Description = "Number of digits in OTP", Group = "Security" },
            new SystemSetting { SettingKey = "COMPANY_NAME", SettingValue = "PolyTrack ERP", Description = "Display name of the organization", Group = "Company" },
            new SystemSetting { SettingKey = "NOTIFY_ADMIN_ON_INQUIRY", SettingValue = "true", Description = "Send notification to admin when inquiry is verified", Group = "System" }
        };

        _context.SystemSettings.AddRange(settings);
        await _context.SaveChangesAsync();

        return Ok("Settings seeded successfully");
    }

    private bool SettingExists(string key) => _context.SystemSettings.Any(e => e.SettingKey == key);
}

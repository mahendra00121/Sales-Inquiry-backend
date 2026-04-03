using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WasteRecordController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WasteRecordController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WasteRecord>>> GetLogs()
    {
        return await _context.WasteRecords
            .Include(w => w.ShopFloorEntry)
            .OrderByDescending(w => w.RecordedAt)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<WasteRecord>> CreateLog(WasteRecord log)
    {
        _context.WasteRecords.Add(log);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLogs), new { id = log.Id }, log);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var record = await _context.WasteRecords.FindAsync(id);
        if (record == null) return NotFound();

        record.ActionTaken = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PackingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PackingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Packing>>> GetPackingRecords()
    {
        return await _context.PackingRecords.Include(p => p.FinalQCRecord).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Packing>> GetPackingRecord(int id)
    {
        var record = await _context.PackingRecords.Include(p => p.FinalQCRecord).FirstOrDefaultAsync(p => p.Id == id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost]
    public async Task<ActionResult<Packing>> CreatePacking(Packing record)
    {
        _context.PackingRecords.Add(record);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPackingRecord), new { id = record.Id }, record);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePackingRecord(int id, Packing record)
    {
        if (id != record.Id) return BadRequest();

        _context.Entry(record).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.PackingRecords.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePackingRecord(int id)
    {
        var record = await _context.PackingRecords.FindAsync(id);
        if (record == null) return NotFound();

        _context.PackingRecords.Remove(record);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

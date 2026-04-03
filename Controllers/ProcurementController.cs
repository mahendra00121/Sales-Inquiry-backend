using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcurementController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProcurementController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Procurement>>> GetProcurements()
    {
        return await _context.Procurements.Include(p => p.ProductionPlan).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Procurement>> GetProcurement(int id)
    {
        var item = await _context.Procurements.Include(p => p.ProductionPlan).FirstOrDefaultAsync(p => p.Id == id);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Procurement>> CreateProcurement(Procurement item)
    {
        _context.Procurements.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProcurement), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProcurement(int id, Procurement item)
    {
        if (id != item.Id) return BadRequest();

        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Procurements.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProcurement(int id)
    {
        var item = await _context.Procurements.FindAsync(id);
        if (item == null) return NotFound();

        _context.Procurements.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

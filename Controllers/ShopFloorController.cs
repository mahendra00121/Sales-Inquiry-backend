using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ShopFloorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ShopFloorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShopFloor>>> GetLogs()
    {
        return await _context.ShopFloorRecords.Include(s => s.ProductionPlan).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShopFloor>> GetLog(int id)
    {
        var record = await _context.ShopFloorRecords.Include(s => s.ProductionPlan).FirstOrDefaultAsync(s => s.Id == id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost]
    public async Task<ActionResult<ShopFloor>> LogOutput(ShopFloor record)
    {
        _context.ShopFloorRecords.Add(record);

        var plan = await _context.ProductionPlans.FindAsync(record.ProductionPlanId);
        if (plan != null)
        {
            plan.CompletedQuantity += record.ActualQuantityProduced;
            if (plan.CompletedQuantity >= plan.TargetQuantity)
            {
                plan.Status = "Completed";
            }
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLog), new { id = record.Id }, record);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLog(int id, ShopFloor record)
    {
        if (id != record.Id) return BadRequest();

        _context.Entry(record).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.ShopFloorRecords.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLog(int id)
    {
        var record = await _context.ShopFloorRecords.FindAsync(id);
        if (record == null) return NotFound();

        _context.ShopFloorRecords.Remove(record);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

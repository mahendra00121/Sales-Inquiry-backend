using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;
using PolyTrack.API.Models;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DispatchController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DispatchController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dispatch>>> GetDispatches()
    {
        return await _context.DispatchRecords.Include(d => d.PackingRecord).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Dispatch>> GetDispatch(int id)
    {
        var dispatch = await _context.DispatchRecords.Include(d => d.PackingRecord).FirstOrDefaultAsync(d => d.Id == id);
        if (dispatch == null) return NotFound();
        return dispatch;
    }

    [HttpPost]
    public async Task<ActionResult<Dispatch>> CreateDispatch(Dispatch dispatch)
    {
        _context.DispatchRecords.Add(dispatch);

        var packing = await _context.PackingRecords.Include(p => p.FinalQCRecord).ThenInclude(f => f.ShopFloorRecord).ThenInclude(s => s.ProductionPlan).ThenInclude(p => p.Order).FirstOrDefaultAsync(p => p.Id == dispatch.PackingId);
        
        if (packing?.FinalQCRecord?.ShopFloorRecord?.ProductionPlan?.Order != null)
        {
            var order = packing.FinalQCRecord.ShopFloorRecord.ProductionPlan.Order;
            order.Status = "Shipped";
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDispatch), new { id = dispatch.Id }, dispatch);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDispatch(int id, Dispatch dispatch)
    {
        if (id != dispatch.Id) return BadRequest();

        _context.Entry(dispatch).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.DispatchRecords.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDispatch(int id)
    {
        var dispatch = await _context.DispatchRecords.FindAsync(id);
        if (dispatch == null) return NotFound();

        _context.DispatchRecords.Remove(dispatch);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
